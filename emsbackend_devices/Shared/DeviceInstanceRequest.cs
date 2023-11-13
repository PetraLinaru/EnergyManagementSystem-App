using System;
namespace emsbackend.Shared
{
	public class DeviceInstanceRequest
	{
        public string ID_User { get; set; }

		public int ID_Dev_Type { get; set; }

        public string Dev_Name { get; set; }

        public string Address { get; set; }


        public DeviceInstanceRequest()
		{
		}
	}
}

