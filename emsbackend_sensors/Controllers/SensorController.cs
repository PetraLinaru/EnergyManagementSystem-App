using System;
namespace emsbackend.Controllers;

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
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

[ApiController]
[Route("api/[controller]")]

//CRUD done by admin only
public class SensorController : ControllerBase
{
    private ISensorService _sensorService;


    public SensorController(ISensorService deviceService)
    {
        _sensorService = deviceService;

    }


    [HttpGet]
    [Route("user-sensor-values")]
    public async Task<IActionResult> GetSensorValuesForUser([FromQuery]SensorValueForUserRequest request)
    {
        var response =await  _sensorService.GetSensorValuesForUser(request);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpGet]
    [Route("user-hourly-consumption")]
    public async Task<IActionResult> GetHourlyConsumptionForUser([FromQuery] SensorValueForUserRequest request)
    {
        var response = await _sensorService.CalculateAndAddHourlyConsumption(request.id, request.ID_Dev_Inst);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }


}
