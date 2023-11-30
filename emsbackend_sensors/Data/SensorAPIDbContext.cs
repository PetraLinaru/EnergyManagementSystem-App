using System;
using System.Reflection.Emit;
using emsbackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace emsbackend.Data
{
	public class SensorAPIDbContext : DbContext
	{

		public SensorAPIDbContext(DbContextOptions<SensorAPIDbContext> options) : base (options)
		{
		}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<UserDevice>()
           .HasKey(userDevice => userDevice.ID_key); // Define the primary key

            builder.Entity<Sensor>()
                .HasKey(sensor => sensor.ID); // Define the primary key for Sensor

            builder.Entity<UserDevice>()
                .HasMany(userDevice => userDevice.Sensors) // One UserDevice has many Sensors
                .WithOne(sensor => sensor.UserDevice) // Each Sensor belongs to one UserDevice
                .HasForeignKey(sensor => sensor.ID_Dev_Inst); // Foreign key property to link UserDevice and Sensor



        }

        public DbSet<Sensor> Sensors{ get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<HourlyConsumption> HourlyConsumptions { get; set; }

    }
}

