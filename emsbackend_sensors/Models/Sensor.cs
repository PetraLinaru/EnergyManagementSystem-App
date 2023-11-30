using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;
using Microsoft.AspNetCore.Identity;

namespace emsbackend.Models
{

    public class Sensor
    {
        [Key]
        public int ID { get; set; }

        public string ID_User { get; set; }

        public int ID_Dev_Inst { get; set; }

        public float Value { get; set; }

        public string Time { get; set; }





        //reference to parent

        public int ID_FKey { get; set; }

        public UserDevice? UserDevice { get; set; }

        public Sensor()
        {
        }
    }

}

