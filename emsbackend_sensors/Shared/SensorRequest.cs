using System;
namespace emsbackend.Shared
{
	public class SensorRequest
	{

        public int ID_Dev_Inst { get; set; }

        public int Value { get; set; }

        public string Time { get; set; }

        public string ID_User { get; set; }

        public SensorRequest()
		{
		}
	}
}

