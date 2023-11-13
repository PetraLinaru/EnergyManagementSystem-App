using System;
using System.Security;
using Microsoft.AspNetCore.Identity;

namespace emsbackend.Models
{
	public class AddUserRequest : IdentityUser
	{
        public string Username { get; set; }

        public string Password { get; set; }

        public string Address { get; set; }
        public AddUserRequest()
		{
		}
	}
}

