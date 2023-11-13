using System;
namespace emsbackend.Controllers;

using System.Net.Http.Headers;
using System.Security;
using System.Text;
using emsbackend.Data;
using emsbackend.Models;
using emsbackend.Services;
using emsbackend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[ApiController]
[Route("api/[controller]")]


//CRUD done by admin only
public class AppUsersController : Controller
{
	private IAppUserService _appUserService;
    public string URL = "http://emsbackend_devices-emsbackend-1:9090/api/Device/create-user";

    public AppUsersController(IAppUserService appUserService)
	{
		_appUserService = appUserService;
	}

	[Route("create-user")]
    [HttpPost]
	public async Task<IActionResult> CreateUser(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _appUserService.RegisterUserAsync(model);

            if (result.IsSuccess)
            {
                var data = new
                {
                    ID_User = result.appUser.Id,
                    Username = result.appUser.UserName
                };
                var json = JsonConvert.SerializeObject(data);
                var data2 = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, URL);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                using var HttpClient = new HttpClient();
                var response = await HttpClient.PostAsync(URL, data2);
                return Ok(response);                //using (var httpClient = new HttpClient())
                //{
                //    var response2 = await httpClient.PostAsync(URL, data);

                //    httpClient.Timeout = TimeSpan.FromSeconds(30);
                //    return Ok(response2);


                //}



            }
            else
                return BadRequest(result);


        }
        return BadRequest("SomeProperties are not valid");
    }

    [Route("update-user")]
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody]StringifiedModel updatedModel)
    {
        if (ModelState.IsValid)
        {
            var result = await _appUserService.UpdateUserAsync(updatedModel);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
                return BadRequest(result);


        }
        return BadRequest("Could not update user, something went wrong");
    }

    [Route("delete-user")]
    [HttpPost]
    public async Task<IActionResult> UpdateUser([FromBody] DeleteModel model)
    {
        if (model.username!=null)
        {
            var result = await _appUserService.DeleteUserAsync(model.username);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
                return BadRequest(result);


        }
        return BadRequest("Could not delete user, something went wrong");
    }

    [Route("all-users")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
     
        var users = await _appUserService.GetAllUsers();
        if (users.IsSuccess)
        {
            List<StringifiedModel> list = users.appUsers;
            return Ok(list);
        }
        return BadRequest("Couldn't find users");
        

    }

    [Route("all-admins")]
    [HttpGet]
    public async Task<IActionResult> GetAllAdmins()
    {

        var users = await _appUserService.GetAllAdmins();
        if (users.IsSuccess)
        {
            List<StringifiedModel> list = users.appUsers;
            return Ok(list);
        }
        return BadRequest("Couldn't find admins");


    }
    [Route("get-user")]
    [HttpGet]
    public async Task<IActionResult> GetUserByUsername([FromBody]string username)
    {
        var user = await _appUserService.GetUserByUsernameAsync(username);
        if(user.IsSuccess)
        {
            return Ok(user);
        }
        return BadRequest("Couldn't find user");
    }


    [AllowAnonymous]
    [Route("update-admin")]
    [HttpPut]
    public async Task<IActionResult> UpdateAdmin([FromBody] UpdateAdminRequest updatedModel)
    {
        if (ModelState.IsValid)
        {
            var result = await _appUserService.UpdateAdminAsync(updatedModel);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
                return BadRequest(result);


        }
        return BadRequest("Could not update user, something went wrong");
    }

    [AllowAnonymous]
    [Route("delete-admin")]
    [HttpDelete]
    public async Task<IActionResult> DeleteAdmin([FromBody] string username)
    {
        if (ModelState.IsValid)
        {
            var result = await _appUserService.DeleteAdminAsync(username);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
                return BadRequest(result);


        }
        return BadRequest("Could not update user, something went wrong");
    }

}

