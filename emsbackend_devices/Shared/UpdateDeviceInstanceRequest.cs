using System;
namespace emsbackend.Shared
{
	public class UpdateDeviceInstanceRequest
	{
        public int ID_Dev_Inst { get; set; }

        public string Address { get; set; }

        public string Dev_Type { get; set; }

        public UpdateDeviceInstanceRequest()
		{
		}
	}
}

