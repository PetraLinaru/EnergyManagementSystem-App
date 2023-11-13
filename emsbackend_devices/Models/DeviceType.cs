using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace emsbackend.Models
{
	public class DeviceType
	{
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID_Dev_Type { get; set; }

        public string Dev_Type_Name { get; set; }

        public int MaxPower { get; set; }

     

        ////one-one

        //public int ID_Dev_Inst { get; set; }

        //public DeviceInstance DeviceInstance { get; set; }

        public DeviceType()
		{
		}
	}
}

