using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using emsbackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Http;
using emsbackend.Services;
using emsbackend.Shared;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace emsbackend.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private IAppUserService _userService;
        public string URL = "http://172.20.0.2/api/Device/create-user";



        public AuthenticateController(IAppUserService userService)
        {

            _userService = userService;
           
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                else
                    return BadRequest(result);

            }
            return BadRequest("Some properties are not valid");
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(model);



                if (result.IsSuccess)
                {
                    return Ok(result);  
                }
                else
                    return BadRequest(result);


            }
            return BadRequest("SomeProperties are not valid");
        }


        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterAdminAsync(model);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                else
                    return BadRequest(result);


            }
            return BadRequest("SomeProperties are not valid");
            
        }

        [HttpGet]
        [Route("get-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            //var users = await userManager.GetUsersInRoleAsync("AppUser");
            //return Ok();


            return Ok();
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            return Ok("Test");
        }
    }
}

