using System;
using System.ComponentModel.DataAnnotations;

namespace emsbackend.Models
{
	public class HourlyConsumption
	{

        [Key]
        public int Id { get; set; }

        public string ID_User { get; set; }

        public int ID_Dev_Inst { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public float Consumption { get; set; }

        public HourlyConsumption()
		{
		}
	}
}

