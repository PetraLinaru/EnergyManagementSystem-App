using System;
using System.ComponentModel.DataAnnotations;
using System.Security;
using Microsoft.AspNetCore.Identity;

namespace emsbackend.Models
{
	public class AppUser :IdentityUser{


		public string User { get; set; }

        public string Password { get; set; }

		public string Address { get; set; }

	
	}
}

