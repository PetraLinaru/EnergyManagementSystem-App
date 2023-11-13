using System;
using emsbackend.Models;
using Microsoft.AspNetCore.Identity;

namespace emsbackend.Shared
{
	public class Response
	{
        internal IEnumerable<string> Error;

        internal bool IsSuccess;

        public string Status { get; set; }

        public string Message { get; set; }

		public string RedirectURL { get; set; }

		public IEnumerable<string> Errors { get; set; }

		public DateTime? ExpireDate { get; set; }
	
		public AppUser appUser { get; set; }

		public List<StringifiedModel> appUsers { get; set; }

		public List<IdentityUser> identityUsers { get; set; }

	
        public Response()
		{
		}
	}
}

