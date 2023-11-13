using System;
using emsbackend.Models;
namespace emsbackend.Shared
{
	public class Response
	{
        internal IEnumerable<string> Error;

        internal bool IsSuccess;

        public string Message { get; set; }

		public string RedirectURL { get; set; }

		public List<DeviceInstance> deviceInstances { get; set; }

		public List<DeviceType> deviceTypes { get; set; }

		public List<UserClone> userClones { get; set; }

		public DeviceType deviceType { get; set; }

		public DeviceInstance deviceInstance { get; set; }
	
        public Response()
		{
		}
	}
}

