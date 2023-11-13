using System;
using System.ComponentModel.DataAnnotations;

namespace emsbackend.Shared
{
	public class UpdateModel
	{
        [Required(ErrorMessage = "ID is required")]
        public Guid AppUserID { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        public string ConfirmPassword { get; set; }

        public string Address { get; set; }


        public UpdateModel()
		{
		}

        
	}
}

