using System;
namespace emsbackend.Models
{
	public class UpdateAppUserRequest { 


    public string Username { get; set; }

    public string Password { get; set; }

    public string Address { get; set; }


    public UpdateAppUserRequest()
		{
		}
	}
}

