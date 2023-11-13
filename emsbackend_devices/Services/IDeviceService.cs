using System;
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
    public interface IDeviceService
    {
        Task<Response> RegisterDeviceInstanceAsync( DeviceInstanceRequest deviceInstanceRequest );

        Task<Response> DeleteDeviceInstanceAsync(string Dev_Name);

        Task<Response> GetAllDeviceInstances();

        Task<Response> GetDeviceInstanceByID_User(string ID_User);

        Task<Response> UpdateDeviceInstanceAsync(UpdateDeviceInstanceRequest model);

        Task<Response> RegisterDeviceTypeAsync(DeviceTypeRequest model);

        Task<Response> UpdateDeviceTypeAsync(UpdateDeviceTypeRequest model);

        Task<Response> DeleteDeviceTypeAsync(String Dev_Type);

        Task<Response> GetAllDeviceTypes();

        Task<Response> GetDeviceTypeByDev_Type_Name(string Dev_Type_Name);

        Task<Response> AddUser(UserRequest request);

        Task<Response> DeleteUser(string username);

        Task<Response> getUsers();
    }

    public class DeviceService : IDeviceService
    {

       

        private DeviceAPIDbContext _dbContext;

       

        public DeviceService(DeviceAPIDbContext dbContext)
        {
            
            _dbContext = dbContext;

        }

        public async Task<Response> DeleteDeviceInstanceAsync(string Dev_Name)
        {
            if (Dev_Name == "")
            {
                return new Response
                {
                    Message = "Empty field for device name"
                };
            }
            var device = await _dbContext.DeviceInstances.FirstAsync<DeviceInstance>(deviceInst=>deviceInst.Dev_Name==Dev_Name);
            if (device == null)
            {
                return new Response
                {
                    Message = "Could't find device with specified name " + Dev_Name,
                    IsSuccess = false
                };

            }

            var result = _dbContext.DeviceInstances.Remove(device);
            

            if (result.State == EntityState.Deleted)
            {
                await _dbContext.SaveChangesAsync();
                return new Response
                {
                    Message = "Device Instance deleted successfully",
                    IsSuccess = true
                    
                };
    
            }
            return new Response
            {
                Message = "Device couldn't be deleted",
                IsSuccess = false

            };
        }

        public async Task<Response> DeleteDeviceTypeAsync(string Dev_Type)
        {
            if (Dev_Type == "")
            {
                return new Response
                {
                    Message = "Invalid input",
                    IsSuccess = false
                };
            }

            var device_type = await _dbContext.DeviceTypes.FirstAsync(device_type => device_type.Dev_Type_Name == Dev_Type);
            
            if (device_type == null)
            {
                return new Response
                {
                    Message = "Couldn't find device type with name:" + Dev_Type,
                    IsSuccess = false
                };
            }

           var result = _dbContext.DeviceTypes.Remove(device_type);


            if (result.State == EntityState.Deleted)
            {
                var deviceInstances = _dbContext.DeviceInstances.Where<DeviceInstance>(deviceInst => deviceInst.ID_Dev_Type == device_type.ID_Dev_Type).ToList();
                foreach(var devInstance in deviceInstances)
                {
                    _dbContext.DeviceInstances.Remove(devInstance);
                }
                await _dbContext.SaveChangesAsync();
                return new Response
                {
                    Message = "Device type deleted successfully",
                    IsSuccess = true,
                };
            }

            return new Response
            {
                Message = "An error occured",
                IsSuccess = false
            };

           
        }
           
        public async Task<Response> GetAllDeviceInstances()
        {
            var result = _dbContext.DeviceInstances.ToList();

            if (result == null)
            {
                return new Response
                {
                    Message = "Couldn't retrieve device instances",
                    IsSuccess = false
                };
            }

            return new Response
            {
                deviceInstances = result,
                Message = "Device instances retrieved successfully",
                IsSuccess = true
            };
        }

        public async Task<Response> GetAllDeviceTypes()
        {
            var result = _dbContext.DeviceTypes.ToList();

            if (result == null)
            {
                return new Response
                {
                    Message = "Couldn't retrieve device types",
                    IsSuccess = false
                };
            }

            return new Response
            {
                deviceTypes=result,
                Message = "Device types retrieved successfully",
                IsSuccess = true
            };
        }

        public async Task<Response> GetDeviceInstanceByID_User(string ID_User)
        {
            if (ID_User == "")
            {
                return new Response
                {
                    Message = "Invalid input",
                    IsSuccess = false
                };
            }

            var result =  _dbContext.DeviceInstances.Where<DeviceInstance>(device => device.ID_User == ID_User).ToList();



            if(result==null)
            {
                return new Response
                {
                    Message = "Couldn't find device instance mapped to user with user id: " + ID_User,
                    IsSuccess = false
                };
            }

            return new Response
            {
                Message = "Device instances retrieved successfully",
                IsSuccess = true,
                deviceInstances = result
            };

            
        }

        public async Task<Response> GetDeviceTypeByDev_Type_Name(string Dev_Type_Name)
        {
            if (Dev_Type_Name == "")
                return new Response
                {
                    Message = "Invalid input",
                    IsSuccess = false
                };

            var DevType = _dbContext.DeviceTypes.Where<DeviceType>(devType => devType.Dev_Type_Name == Dev_Type_Name).First();

            if (DevType == null)
            {
                return new Response
                {
                    Message = "Couldn't find device type",
                    IsSuccess = false

                };
            }

            return new Response
            {
                Message = "Device etype found successfully",
                IsSuccess = true,
                deviceType=DevType

            };

        }

        public async Task<Response> RegisterDeviceInstanceAsync( DeviceInstanceRequest deviceInstanceRequest)
        {
            if (deviceInstanceRequest == null)
            {
                return new Response
                {
                    Message = "Invalid input",
                    IsSuccess = false
                };
            }
            var user = await _dbContext.UserClones.FindAsync(deviceInstanceRequest.ID_User);
            if(user==null)
            {
                return new Response
                {
                    Message = "Couldn't find user",
                    IsSuccess = false
                };
            }
            var device_type = await _dbContext.DeviceTypes.FindAsync(deviceInstanceRequest.ID_Dev_Type);
            if (device_type == null)
            {
                return new Response
                {
                    Message = "Couldn't find device_type",
                    IsSuccess = false
                };
            }
        
            int devicesCount =  await _dbContext.DeviceInstances.CountAsync(d => d.ID_Dev_Type == deviceInstanceRequest.ID_Dev_Type) + 1;

            // Generate a unique name for the new DeviceInstance
            string deviceInstanceName = deviceInstanceRequest.Dev_Name + "_" + device_type.Dev_Type_Name + "_" + devicesCount;

            var alreadyExists = await _dbContext.DeviceInstances.AnyAsync(d => d.Dev_Name == deviceInstanceName);


            if (!alreadyExists)
            {

                //here we have the NULLABLE PROPERTIES
                // we use nullable for userclone and devicetype instances because we don t want to create new user /devicetypes
                //we want to map a device inst to a device type and a user
                
                var newDeviceInstance = new DeviceInstance
                {
                    Dev_Name = deviceInstanceRequest.Dev_Name + "_" + device_type.Dev_Type_Name + "_" + devicesCount,
                    Address = deviceInstanceRequest.Address,
                   // DeviceType = device_type,  those are like Direct Object Reference, which means each time i create a new device instance, a new user/devicetype gets created
                   // UserClone = user,             and i get an error saying that i can t create duplicates
                    ID_Dev_Type = device_type.ID_Dev_Type, // those are foreign keys, so they're oke
                    ID_User = user.ID_User
                

                };




                user.DeviceInstances.Add(newDeviceInstance);

                _dbContext.DeviceInstances.Add(newDeviceInstance);
                


                var result = await _dbContext.SaveChangesAsync();


                if (result > 0)
                {

                    return new Response
                    {
                        Message = "Device instance added successfully",
                        IsSuccess = true,
                        deviceInstance=newDeviceInstance
                    };
                }

            }

            return new Response
            {
                Message = "Couldn't add device instance",
                IsSuccess = false
            };


        }

        public async Task<Response> RegisterDeviceTypeAsync(DeviceTypeRequest deviceTypeRequest)
        {
            if (deviceTypeRequest == null)
            {
                return new Response
                {
                    Message = "Invalid input",
                    IsSuccess = false
                };
            }
            var alreadyExists =  await _dbContext.DeviceTypes.AnyAsync<DeviceType>(d => d.Dev_Type_Name == deviceTypeRequest.Dev_Type_Name && d.MaxPower == deviceTypeRequest.MaxPower);

            if (!alreadyExists)
            {

                DeviceType deviceType = new DeviceType
                {
                    Dev_Type_Name = deviceTypeRequest.Dev_Type_Name,
                    MaxPower = deviceTypeRequest.MaxPower
                };

                var result = await _dbContext.DeviceTypes.AddAsync(deviceType);
                var result2 = await _dbContext.SaveChangesAsync();
                Console.Write(result2.ToString());


                if (result.State == EntityState.Unchanged)
                {
                    return new Response
                    {
                        Message = "Device Type created successfully!",
                        IsSuccess = true,
                        deviceType=deviceType
                    };
                }

            }

            

            return new Response
            {
                Message = "A problem occured, couldn't create device type",
                IsSuccess = false

            };
        }

        public async Task<Response> UpdateDeviceInstanceAsync(UpdateDeviceInstanceRequest model)
        {
            if (model == null)
            {
                return new Response
                {
                    Message = "Invalid input",
                    IsSuccess = false
                };
            }
            var deviceInstance = await _dbContext.DeviceInstances.FindAsync(model.ID_Dev_Inst);

            if (deviceInstance == null)
            {
                return new Response
                {
                    Message = "Could not find device Instance",
                    IsSuccess = false
                };
            }

            var oldDevType = _dbContext.DeviceTypes.Where(dt => dt.ID_Dev_Type == deviceInstance.ID_Dev_Type).First();

            var newDeviceType = _dbContext.DeviceTypes.Where(deviceType => deviceType.Dev_Type_Name == model.Dev_Type).First();
            
            deviceInstance.Dev_Name=deviceInstance.Dev_Name.Replace(oldDevType.Dev_Type_Name, model.Dev_Type);
            deviceInstance.ID_Dev_Type = newDeviceType.ID_Dev_Type;
            deviceInstance.Address = model.Address;

            var result = _dbContext.DeviceInstances.Update(deviceInstance);

            if (result.State == EntityState.Modified)
            {
                await _dbContext.SaveChangesAsync();
                return new Response
                {
                    Message = "Device instance updated successfully",
                    IsSuccess = true,
                    deviceInstance=deviceInstance
                };
            }
            return new Response
            {
                Message = "Couldn't update device instance",
                IsSuccess = false
            };
        }

        public async Task<Response> UpdateDeviceTypeAsync(UpdateDeviceTypeRequest model)
        {
            if (model == null)
            {
                return new Response
                {
                    Message = "Invalid input",
                    IsSuccess = false
                };
            }

            var deviceTypeToUpdate = _dbContext.DeviceTypes.Where(dt => dt.ID_Dev_Type == model.ID_Dev_Type).First();

            if (deviceTypeToUpdate == null)
            {
                return new Response
                {
                    Message = "Couldn't find device type",
                    IsSuccess = false
                };
            }
            

            var oldDTName = deviceTypeToUpdate.Dev_Type_Name;

            deviceTypeToUpdate.Dev_Type_Name = model.Dev_Type_Name;

            deviceTypeToUpdate.MaxPower = model.MaxPower;

            var result1 = _dbContext.DeviceTypes.Update(deviceTypeToUpdate);

            var oldDeviceInstances = _dbContext.DeviceInstances.Where(dt => dt.ID_Dev_Type == model.ID_Dev_Type).ToList();

            if (oldDeviceInstances != null)
            {
                foreach (var oldDevInstance in oldDeviceInstances)
                {
                    oldDevInstance.Dev_Name = oldDevInstance.Dev_Name.Replace(oldDTName, model.Dev_Type_Name);
                    _dbContext.DeviceInstances.Update(oldDevInstance);
                }
            }

            if (result1.State == EntityState.Modified)
            {
                await _dbContext.SaveChangesAsync();
                return new Response
                {
                    Message = "Device type and associated device instances updated successfully!",
                    IsSuccess = true,
                    deviceType = deviceTypeToUpdate,
                    deviceInstances = oldDeviceInstances
                };
            }

            return new Response
            {
                Message = "Couldn't update device type",
                IsSuccess = false

            };
        }

        public async Task<Response> AddUser(UserRequest request)
        {
            if (request == null)
            {
                return new Response
                {
                    Message = "Invalid input",
                    IsSuccess = false

                };
            }

            UserClone user = new UserClone
            {
                ID_User = request.ID_User,
                Username = request.Username,
            };

            var result = await _dbContext.UserClones.AddAsync(user);
            

            if (result.State == EntityState.Added)
            {
                await _dbContext.SaveChangesAsync();
                return new Response
                {
                    Message = "User added successfully",
                    IsSuccess = true
                    
                };
            }
            return new Response
            {
                Message = "Could not add user",
                IsSuccess = false

            };
        }

        public async Task<Response> DeleteUser(string username)
        {
            if (username == "")
            {
                return new Response
                {
                    Message = "Invalid input",
                    IsSuccess = false
                };
            }

            var toDelete = _dbContext.UserClones.Where(u => u.Username == username).First();

            var result = _dbContext.UserClones.Remove(toDelete);

            if (result.State == EntityState.Deleted)
            {
                await _dbContext.SaveChangesAsync();
                return new Response
                {
                    Message = "User deleted successfully",
                    IsSuccess = true
                };
            }

            return new Response
            {
                Message = "Could not delete user",
                IsSuccess = false

            };
        }


        public async Task<Response> getUsers()
        {
            List<UserClone> array=_dbContext.UserClones.ToList();

            if (array == null)
            {
                return new Response
                {
                    Message = "No users found",
                    IsSuccess = false
                };
            }

            return new Response
            {
                Message = "Users found",
                IsSuccess = true,
                userClones=array
            };
        }


    }
}
