using System;
namespace emsbackend.Shared
{
	public class UserDeviceRequest
	{
        public string ID_User { get; set; }

        public int ID_Dev_Inst { get; set; }

        public int ID_Dev_Type { get; set; }

        public float MaxValue { get; set; }

        public UserDeviceRequest()
		{
		}
	}
}

