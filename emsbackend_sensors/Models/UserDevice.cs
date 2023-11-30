using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace emsbackend.Models
{
	public class UserDevice
	{

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_key { get; set; }

        public string ID_User { get; set; }

        public int ID_Dev_Inst { get; set; }
        
        public int ID_Dev_Type { get; set; }

        public float MaxValue { get; set; }

        //one to many
        public ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();

        public UserDevice()
		{

		}

       
	}
}

