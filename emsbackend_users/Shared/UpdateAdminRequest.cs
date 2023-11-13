using System;
namespace emsbackend.Shared
{
	public class UpdateAdminRequest
	{

		public string Id { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public string OldPassword { get; set; }

		public string Email { get; set; }


		public UpdateAdminRequest()
		{
		}
	}
}

