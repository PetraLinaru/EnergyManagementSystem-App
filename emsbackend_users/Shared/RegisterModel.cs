using System;
using System.ComponentModel.DataAnnotations;

namespace emsbackend.Shared
{
	public class RegisterModel
	{
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        public RegisterModel()
		{
            
        }
	}
}

