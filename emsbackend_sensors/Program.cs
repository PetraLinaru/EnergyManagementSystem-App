using System;
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
builder.Services.AddDbContext<SensorAPIDbContext>(opt => opt.UseInMemoryDatabase("emsbackend_sensors"));
builder.Services.AddScoped<SensorAPIDbContext>();
builder.Services.AddHostedService<RabbitMQConsumerService>();


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





//Add ISensorService
builder.Services.AddScoped<ISensorService, SensorService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();


app.UseCors(x =>x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000"));


app.MapControllers();


app.Run();

