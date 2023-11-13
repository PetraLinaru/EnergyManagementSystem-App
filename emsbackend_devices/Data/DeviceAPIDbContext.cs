using System;
using System.Reflection.Emit;
using emsbackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace emsbackend.Data
{
	public class DeviceAPIDbContext : DbContext
	{

		public DeviceAPIDbContext(DbContextOptions<DeviceAPIDbContext> options) : base (options)
		{
		}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<UserClone>()
             .HasMany(user => user.DeviceInstances) // One UserClone has many DeviceInstances
             .WithOne(deviceInstance=>deviceInstance.UserClone) // Each DeviceInstance belongs to one UserClone
             .HasForeignKey(deviceInstance => deviceInstance.ID_User); // Foreign key property to userclone on ID_USER

            //one device instance has a device type
            builder.Entity<DeviceInstance>()
            .HasOne(deviceInstance=>deviceInstance.DeviceType) //one device has one device type
            .WithOne() //i don t really wanna say that one device type has one device inst 
            .HasForeignKey<DeviceType>(deviceInstance => deviceInstance.ID_Dev_Type); // foreign key to device instance on ID_DEV_TYPE
        }

        public DbSet<DeviceInstance> DeviceInstances { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<UserClone> UserClones { get; set; }
    }
}

