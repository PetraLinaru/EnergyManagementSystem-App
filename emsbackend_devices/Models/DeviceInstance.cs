using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;
using Microsoft.AspNetCore.Identity;

namespace emsbackend.Models
{

    public class DeviceInstance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Dev_Inst { get; set; }

        public string Dev_Name { get; set; }

        public string Address { get; set; }



        //reference to parent

        public string ID_User { get; set; }//NULLABLE

        public UserClone? UserClone { get; set; } //NULLABLE


        //reference to devicetype (children)
        //one-to-one
        public int ID_Dev_Type { get; set; } //NULLABLE

        public DeviceType? DeviceType { get; set; } //NULLABLE

        public DeviceInstance()
        {
        }
    }

}

