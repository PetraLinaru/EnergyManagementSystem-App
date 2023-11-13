using System;
namespace emsbackend.Shared
{
	public class UpdateDeviceTypeRequest
	{
        public int ID_Dev_Type { get; set; }

        public string Dev_Type_Name { get; set; }

        public int MaxPower { get; set; }

        public UpdateDeviceTypeRequest()
		{
		}
	}
}

