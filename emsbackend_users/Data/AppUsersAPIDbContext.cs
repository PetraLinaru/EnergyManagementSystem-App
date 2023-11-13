using System;
using System.Reflection.Emit;
using emsbackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace emsbackend.Data
{
	public class AppUsersAPIDbContext : IdentityDbContext
	{

		public AppUsersAPIDbContext(DbContextOptions<AppUsersAPIDbContext> options) : base (options)
		{
		}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityUserRole<Guid>>().HasKey(p => new { p.UserId, p.RoleId });
            
        }

        public DbSet<AppUser> AppUsers { get; set; }
	}
}

