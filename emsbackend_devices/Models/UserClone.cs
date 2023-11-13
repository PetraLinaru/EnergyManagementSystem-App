using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace emsbackend.Models
{
	public class UserClone
	{
  
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string ID_User { get; set; }

        public string Username { get; set; }


        //one to many
        public ICollection<DeviceInstance> DeviceInstances { get; } = new List<DeviceInstance>();

        public UserClone()
		{
           

		}
	}
}

