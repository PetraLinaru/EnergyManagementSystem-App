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

    private IRabbitMQProducerService _rabbitMQProducerService;

    public string URL = "http://emsbackend_devices-emsbackend-1:9090/api/Device/create-user";

    public AppUsersController(IAppUserService appUserService, IRabbitMQProducerService rabbitMQProducerService)
	{
		_appUserService = appUserService;

        _rabbitMQProducerService = rabbitMQProducerService;

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
                var intermediary = await _appUserService.GetUserByUsernameAsync(model.Username);
                var innerResult = await _rabbitMQProducerService.ProduceAsync(intermediary.appUser.Id);
                
                if (innerResult.IsSuccess == true)
                {
                    return Ok(result);
                }
                else
                    return BadRequest("Something went wrong");
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

