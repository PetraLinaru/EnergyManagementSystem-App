using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using emsbackend.Data;
using emsbackend.Models;
using emsbackend.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;


namespace emsbackend.Services
{
    public interface IAppUserService
	{
		Task<Response> RegisterUserAsync(RegisterModel model);

		Task<Response> RegisterAdminAsync(RegisterModel model);

		Task<Response> LoginUserAsync(LoginModel model);

		Task<Response> UpdateUserAsync(StringifiedModel model);

		Task<Response> DeleteUserAsync(String username);

		Task<Response> GetAllUsers();

		Task<Response> GetUserByUsernameAsync(string username);

		Task<Response> GetAllAdmins();

		Task<Response> UpdateAdminAsync(UpdateAdminRequest request);

        Task<Response> DeleteAdminAsync(String username);
        
        }

	public class AppUserService : IAppUserService
	{

		private UserManager<IdentityUser> _userManager;

		private RoleManager<IdentityRole> _roleManager;

		private AppUsersAPIDbContext _context;
 
        private IConfiguration _configuration;

		public AppUserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, AppUsersAPIDbContext dbContext)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
			_context = dbContext;		
			
		}

		public async Task<Response> RegisterUserAsync(RegisterModel model)
		{
			if (model == null)
				throw new NullReferenceException("RegisterModel is Null");
			if (model.Password != model.ConfirmPassword)
				return new Response
				{
					Message = "Confirm password doesn't match the password",
					Status = "Error",
					IsSuccess = false
				};

			var generatedGuid = Guid.NewGuid();

            var identityUser = new AppUser
			{
                Id = generatedGuid.ToString(),
                Email = model.Email,
				UserName = model.Username,
				Password = model.Password,
				Address = model.Address,
				User =  model.Username,
			
				
			};

			var result = await _userManager.CreateAsync(identityUser, model.Password);

			if(!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }
            
            await _userManager.AddToRoleAsync(identityUser, "User");

		

			if(result.Succeeded)
			{
				return new Response
				{
					Message = "User created successfully!",
					RedirectURL = "/user",
					IsSuccess=true,
				    appUser = identityUser
                };
			}
			return new Response
			{
				Message = "User did not create",
				IsSuccess = false,
                Error = result.Errors.Select(e => e.Description)
			};
        }


		public async Task<Response> RegisterAdminAsync(RegisterModel model)
		{
            if (model == null)
                throw new NullReferenceException("RegisterModel is Null");
            if (model.Password != model.ConfirmPassword)
                return new Response
                {
                    Message = "Confirm password doesn't match the password",
                    Status = "Error",
                    IsSuccess = false,
					
                };


            var identityUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Username,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            await _userManager.AddToRoleAsync(identityUser, "Admin");

            if (result.Succeeded)
            {
                return new Response
                {
                    Message = "Admin created successfully!",
                    RedirectURL = "/admin",
                    IsSuccess = true
                };
            }
            return new Response
            {
                Message = "Admin did not create",
                IsSuccess = false,
                Error = result.Errors.Select(e => e.Description)
            };
        }

		public async Task<Response> LoginUserAsync(LoginModel model)
		{
			var user = await _userManager.FindByNameAsync(model.Username);
			if (user == null)
			{
				return new Response
				{
					Message = "There's no user with that email address",
					IsSuccess = false
				};

			}

			var result = await _userManager.CheckPasswordAsync(user, model.Password);
			if (!result)
			{
				return new Response
				{
					Message = "There's no user with that password",
					IsSuccess = false
				};

			}

            var role = await _userManager.GetRolesAsync(user);
	

			var claims = new[]
			{
				new Claim("Username",model.Username),
				new Claim(ClaimTypes.NameIdentifier,user.Id),
				new Claim(ClaimTypes.Role,role[0].ToString())
			
			};

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
					expires: DateTime.Now.AddHours(3),
					claims: claims,
					signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
					);

			
			

			var redirectURL = "";

			if (role.Contains("Admin"))
			{
                redirectURL = "/adminhome";
			}
			if(role.Contains("User"))
			{
				redirectURL = "/user";
			}
			string tokenizedString = new JwtSecurityTokenHandler().WriteToken(token);


			Console.WriteLine(tokenizedString);

			return new Response
			{
				Message = tokenizedString,
				IsSuccess = true,
				ExpireDate = token.ValidTo,
				RedirectURL = redirectURL,
				appUser = new AppUser
				{
					Id = user.Id,
                    User = model.Username

				}
			};

		}

		public async Task<Response> UpdateUserAsync(StringifiedModel model)
		{
			var user = await _userManager.FindByIdAsync(model.AppUserID);

			var user2 =  _context.AppUsers.Find(model.AppUserID);

			IdentityUser identityUser= await _userManager.FindByIdAsync(model.AppUserID);
			if (user == null)
			{
				return new Response
				{
					Message = "Couldn't find user to update",
					IsSuccess = false,
					appUser = new AppUser
					{
						User = model.Username,
						Password = model.Password,
						Email = model.Email,

					}
				};

			}

			

            user2.UserName = model.Username;
			user2.Email = model.Email;
			user2.Address = model.Address;
			user2.User = model.Username;
			await _userManager.ChangePasswordAsync(user2,model.oldPassword,model.Password);
			
		  
                
            var result = await _userManager.UpdateAsync(user2);
			await _context.SaveChangesAsync();

			//await _userManager.UpdateAsync(identityUser);




			if (result == null)
			{
				return new Response
				{
					Message = "User update failed",
					IsSuccess = false
				};
			}



            return new Response
			{
				Message = "User updated successfully",
				IsSuccess = true
			};
        }

        public async Task<Response> DeleteUserAsync(String username)
        {
			var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return new Response
                {
                    Message = "Couldn't find user to delete",
                    IsSuccess = false
                };

            }

            IdentityResult result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return new Response
                {
                    Message = "User delete failed",
                    IsSuccess = false
                };
            }
            return new Response
            {
                Message = "User deleted successfully",
                IsSuccess = true
            };
        }

		public async Task<Response> GetAllUsers()
		{
			List<AppUser> users = _context.AppUsers.ToList();



			if (users.Count()==0)
			{
				return new Response
				{
					Message = "Could not retrieve users",
					IsSuccess = false
				};

			}
			else
			{
				List<StringifiedModel> appUsers = new List<StringifiedModel>();

                foreach (var identityUser in users)
                {
					StringifiedModel appUser = new StringifiedModel
					{
                        
                        Username = identityUser.UserName,
                        Email = identityUser.Email,
						AppUserID=identityUser.Id,
						Address=identityUser.Address
					    
						
                    };

					appUsers.Add(appUser);

					

                }
				return new Response
				{
					Message = "Users retrieved succesfully",
					appUsers = appUsers,
					IsSuccess = true

				};
			}

        }

		public async Task<Response> GetUserByUsernameAsync(string username)
		{
			var user = await _userManager.FindByNameAsync(username);
			if(user==null)
			{
				return new Response
				{
					Message = "Couldn't find user with username" + username,
					IsSuccess = false

				};
			}

			return new Response
			{
				Message = "Found user!",
				IsSuccess=true,
				appUser = new AppUser
				{
					Id=user.Id,
					Email = user.Email,
					User = user.UserName
				}
			};
		}

		public async Task<Response> GetAllAdmins()
		{

            var admins =  _userManager.GetUsersInRoleAsync("Admin").Result;

            List<StringifiedModel> appUsers = new List<StringifiedModel>();

            foreach (var identityUser in admins)
            {
                StringifiedModel appUser = new StringifiedModel
                {

                    Username = identityUser.UserName,
                    Email = identityUser.Email,
                    AppUserID = identityUser.Id,


                };

                appUsers.Add(appUser);



            }
            return new Response
            {
                Message = "Admins retrieved succesfully",
                appUsers = appUsers,
                IsSuccess = true

            };

        }

		public async Task<Response> UpdateAdminAsync(UpdateAdminRequest request)
		{


			var admins = _userManager.GetUsersInRoleAsync("Admin").Result;

			if (admins == null)
			{
                return new Response
                {
                    Message = "Couldn't find admin to update",
                    IsSuccess = true

                };
            }

		

			foreach(var admin in admins)
			{
				if (admin.Id == request.Id)
				{

                    admin.UserName = request.Username;
                    admin.Email = request.Email;
                    await _userManager.ChangePasswordAsync(admin, request.OldPassword, request.Password);
                    var result = await _userManager.UpdateAsync(admin);

					if (result == null)
					{
						return new Response
						{
							Message = "Admin update failed",
							IsSuccess = false
						};
					}

					return new Response
					{
						Message = "Admin updated successfully",
						IsSuccess = true,
						appUser = new AppUser
						{
							User = admin.UserName,
							Email = admin.Email,

						}
					};



				}
            }

            return new Response
            {
                Message = "Couldn't find admin to update",
                IsSuccess = true

            };
        }


        public async Task<Response> DeleteAdminAsync(String username)
        {
            var admins = _userManager.GetUsersInRoleAsync("Admin").Result;
            if (admins == null)
            {
                return new Response
                {
                    Message = "Couldn't find user to delete",
                    IsSuccess = false
                };

            }

			foreach (var admin in admins)
			{
				if (admin.UserName == username)
				{
					IdentityResult result = await _userManager.DeleteAsync(admin);

					if (!result.Succeeded)
					{
						return new Response
						{
							Message = "Admin delete failed",
							IsSuccess = false
						};
					}
					return new Response
					{
						Message = "Admin deleted successfully",
						IsSuccess = true
					};
				}
			}

            return new Response
            {
                Message = "Couldn't find user to delete",
                IsSuccess = false
            };
        }

    }

}

