using System.Net;
using System.Text;
using emsbackend.Data;
using emsbackend.Models;
using emsbackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Entity Framework with SQL Server
builder.Services.AddDbContext<DeviceAPIDbContext>(options =>
   {
       options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
       options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
   
   });


// Adding Authentication
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    //   options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//});

// Adding Jwt Bearer
//.AddJwtBearer(options =>
//{
//    options.SaveToken = true;
//    options.RequireHttpsMetadata = false;
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ValidateIssuerSigningKey = true,
//        ValidateLifetime=true,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
//    };
//});
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowOrigin", builder => {
        builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowCredentials()
            .WithHeaders("Authorization", "Content-Type", "access-control-allow-origin")
            .WithExposedHeaders("Authorization")
            .AllowAnyMethod();
    });
});




//Add IUserService
builder.Services.AddScoped<IDeviceService, DeviceService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();

app.UseCors(x =>x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000"));

//app.UseAuthentication();

//app.UseAuthorization();

app.MapControllers();




//Role Manager

//using (var scope = app.Services.CreateScope())
//{
//    var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//    var roles = new[] { "Admin", "User" };

//    foreach(var role in roles)
//    {
//        if (!await RoleManager.RoleExistsAsync(role))
//        {
//            await RoleManager.CreateAsync(new IdentityRole(role));
//        }
//    }
//}


app.Run();

