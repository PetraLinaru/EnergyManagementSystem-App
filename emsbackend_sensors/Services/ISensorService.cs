using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using emsbackend.Data;
using emsbackend.Models;
using emsbackend.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;


namespace emsbackend.Services
{
    public interface ISensorService
    {
        Task<Response> RegisterSensorValue( SensorRequest sensorRequest );

        Task<Response> GetSensorValues();

        Task<Response> GetSensorValuesForUser(SensorValueForUserRequest request);

        Task<Response> CalculateAndAddHourlyConsumption(string userID, int deviceInstanceID);
    }

    public class SensorService : ISensorService
    {
        private SensorAPIDbContext _dbContext;
        

        public SensorService(SensorAPIDbContext dbContext)
        {

            _dbContext = dbContext;

        }

        public async Task<Response> RegisterSensorValue(SensorRequest sensorRequest)
        {
            if (sensorRequest == null)
            {
                return new Response
                {
                    Message = "Invalid input",
                    IsSuccess = false
                };
            }

            Sensor sensor = new Sensor
            {
                ID_Dev_Inst = sensorRequest.ID_Dev_Inst,
                Time = sensorRequest.Time,
                ID_User = sensorRequest.ID_User
            };

            var result = await _dbContext.Sensors.AddAsync(sensor);

            if (result.State == EntityState.Added)
            {
               await _dbContext.SaveChangesAsync();
                return new Response
                {
                    Message = "Sensor added successfully",
                    Sensors = _dbContext.Sensors.ToList(),
                    sensor = sensor,
                    IsSuccess=true
                };
            }

            return new Response
            {
                Message = "Couldn t add sensor",
                IsSuccess = false
            };
           
        }

        public async Task<Response> GetSensorValues()
        {
            List<Sensor> response = _dbContext.Sensors.ToList();
            if (response != null)
            {
                return new Response
                {
                    Message = "Got em",
                    Sensors = response,
                    IsSuccess = true
                };
            }
            else
            {
                return new Response
                {
                    Message = "Encountered a problem",
                    IsSuccess = false
                };
            }
        }

        public async Task<Response> GetSensorValuesForUser(SensorValueForUserRequest request)
        {
            List<Sensor> sensorValues = _dbContext.Sensors.ToList();

            List<Sensor> toReturn = new List<Sensor>();

            foreach (var sensor in sensorValues)
            {
                if (sensor.ID_User == request.id && sensor.ID_Dev_Inst == request.ID_Dev_Inst)
                {
                    toReturn.Add(sensor);
                }
            }
            if (sensorValues == null)
            {
                return new Response
                {
                    Message = "No registered data available",
                    IsSuccess = true
                };
            }
            return new Response
            {
                Message = "Found sensor values",
                IsSuccess = true,
                Sensors = toReturn

            };


        }

        public async Task<Response> CalculateAndAddHourlyConsumption(string userID, int deviceInstanceID)
        {
            List<string> consumptionData = new List<string>();
            List<string> consumptionDataNotReady = new List<string>
            {
                "Not enough data yet!"
            };
            var sensorData = _dbContext.Sensors
                .Where(s => s.ID_User == userID && s.ID_Dev_Inst == deviceInstanceID)
                .OrderBy(s => s.Time)
                .ToList();

            var groupedData = sensorData
                .GroupBy(s => DateTime.ParseExact(s.Time, "MM/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture).Hour);

            if (sensorData.Count() < 1)
            {
                return new Response
                {
                    Messages = consumptionDataNotReady,
                    IsSuccess = true
                };
            }
            else
            {
                foreach (var group in groupedData)
                {
                    var groupList = group.ToList();
                    var hourlyConsumption = new HourlyConsumption
                    {
                        ID_User = userID,
                        ID_Dev_Inst = deviceInstanceID,
                        StartTime = DateTime.ParseExact(groupList.First().Time, "MM/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                        EndTime = DateTime.ParseExact(groupList.Last().Time, "MM/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                        Consumption = groupList.Last().Value - groupList.First().Value
                    };

                    var result = await _dbContext.HourlyConsumptions.AddAsync(hourlyConsumption);

                    if (result.State != EntityState.Added)
                    {
                        return new Response
                        {
                            Message = "Something went wrong",
                            IsSuccess = false,
                        };
                    }

                    Console.WriteLine($"Hourly consumption for interval {hourlyConsumption.StartTime} - {hourlyConsumption.EndTime}: {hourlyConsumption.Consumption}");
                    consumptionData.Add($"Hourly consumption for interval {hourlyConsumption.StartTime} - {hourlyConsumption.EndTime}: {hourlyConsumption.Consumption}");
                }
            }

            return new Response
            {
                Messages = consumptionData,
                IsSuccess = true
            };
        }

    }
}

