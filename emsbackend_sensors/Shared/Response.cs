using System;
using emsbackend.Models;
namespace emsbackend.Shared
{
	public class Response
	{
        internal IEnumerable<string> Error;

        internal bool IsSuccess;

        public string Message { get; set; }

        public List<string> Messages { get; set; }

        public string RedirectURL { get; set; }

		public List<Sensor> Sensors { get; set; }

		public Sensor sensor { set; get; }

        public Response()
		{
		}
	}
}

