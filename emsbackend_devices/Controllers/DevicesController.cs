using System;
namespace emsbackend.Controllers;

using System.Security;
using emsbackend.Data;
using emsbackend.Models;
using emsbackend.Services;
using emsbackend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]

//CRUD done by admin only
public class DeviceController : ControllerBase
{
	private IDeviceService _deviceService;

	public DeviceController(IDeviceService deviceService)
	{
        _deviceService = deviceService;
	}

	[Route("create-device-instance")]
    [HttpPost]
	public async Task<IActionResult> CreateDeviceInstance([FromBody] DeviceInstanceRequest model)
    {
        if (ModelState.IsValid)
        {
            var result = await _deviceService.RegisterDeviceInstanceAsync(model);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
                return BadRequest(result);


        }
        return BadRequest("SomeProperties are not valid");
    }

    [Route("create-device-type")]
    [HttpPost]
    public async Task<IActionResult> CreateDeviceType([FromBody]DeviceTypeRequest model)
    {
        if (ModelState.IsValid)
        {
            var result = await _deviceService.RegisterDeviceTypeAsync(model);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
                return BadRequest(result);


        }
        return BadRequest("Some properties are not valid");
    }

    [Route("create-user")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody]UserRequest user)
    {
        if (ModelState.IsValid)
        {
            var result = await _deviceService.AddUser(user);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        return BadRequest("Some properties are not valid");
    }

    [Route("delete-user")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromBody] string user)
    {
        if (ModelState.IsValid)
        {
            var result = await _deviceService.DeleteUser(user);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        return BadRequest("Some properties are not valid");
    }



    [Route("delete-dev-type")]
    [HttpDelete]
    public async Task<IActionResult> DeleteDeviceType([FromBody]string Dev_Type)
    {
        if (ModelState.IsValid)
        {
            var result = await _deviceService.DeleteDeviceTypeAsync(Dev_Type);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        return BadRequest("Some properties are not valid");
    }

    [Route("delete-dev-inst")]
    [HttpDelete]
    public async Task<IActionResult> DeleteDeviceInstance([FromBody]string Dev_Name)
    {
        if (ModelState.IsValid)
        {
            var result = await _deviceService.DeleteDeviceInstanceAsync(Dev_Name);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        return BadRequest("Some properties are not valid");
    }


    [Route("all-dev-types")]
    [HttpGet]
    public async Task<ActionResult<Response>> GetAllDeviceTypes()
    {
        if (ModelState.IsValid)
        {
            var result = await _deviceService.GetAllDeviceTypes();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        return BadRequest();
    }


    [Route("all-dev-inst")]
    [HttpGet]
    public async Task<IActionResult> GetAllDeviceInstances()
    {
        var result = await _deviceService.GetAllDeviceInstances();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [Route("update-device-instance")]
    [HttpPut]
    public async Task<IActionResult> UpdateDeviceInstance([FromBody]UpdateDeviceInstanceRequest request)
    {
        if (request != null)
        {
            var result = await _deviceService.UpdateDeviceInstanceAsync(request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        return BadRequest("Invalid input");
    }

    [Route("update-device-type")]
    [HttpPut]
    public async Task<IActionResult> UpdateDeviceType([FromBody]UpdateDeviceTypeRequest request)
    {
        if (request != null)
        {
            var result = await _deviceService.UpdateDeviceTypeAsync(request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        return BadRequest("Invalid input");
    }

    [Route("get-dev-instance/{ID_User}")]
    [HttpGet]
    public async Task<IActionResult> GetDeviceInstanceByID_User([FromRoute] string ID_User)
    {
        if (ID_User != "")
        {
            var result = await _deviceService.GetDeviceInstanceByID_User(ID_User);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        return BadRequest("Invalid input");
    }

    [Route("get-dev-type/{Dev_Type_Name}")]
    [HttpGet]
    public async Task<IActionResult> GetDeviceTypeByDev_Type_Name([FromRoute]string Dev_Type_Name)
    {
        if (Dev_Type_Name != "")
        {
            var result = await _deviceService.GetDeviceTypeByDev_Type_Name(Dev_Type_Name);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        return BadRequest("Invalid input");
    }


    [Route("all-users")]
    [HttpGet]
    public async Task<IActionResult> GetUserClonesExistent()
    {
        var result = await _deviceService.getUsers();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}

