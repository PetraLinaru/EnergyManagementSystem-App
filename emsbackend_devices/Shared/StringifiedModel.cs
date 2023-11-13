using System;
using System.ComponentModel.DataAnnotations;

namespace emsbackend.Shared
{
    public class StringifiedModel
    {

        //de la vechiul proiect, ignore for now
        
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Old Password is required")]
        public string oldPassword { get; set; }

        public string Address { get; set; }

        public StringifiedModel()
        {
        }
    }
}

