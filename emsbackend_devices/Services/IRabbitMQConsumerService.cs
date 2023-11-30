using System;
using System.Diagnostics.Tracing;
using System.Text;
using Docker.DotNet.Models;
using emsbackend.Data;
using emsbackend.Models;
using emsbackend.Shared;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace emsbackend.Services
{
    public interface IRabbitMQConsumerService
    {
        public void ConsumeAsync();
    }

    public class RabbitMQConsumerService : BackgroundService
    {

     
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private IModel _deviceChannel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQConsumerService( ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
        {
            this._logger = loggerFactory.CreateLogger<RabbitMQConsumerService>();
            this._serviceScopeFactory = serviceScopeFactory;
            InitRabbitMQ();
        }

  
        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@rabbitmq2") };


            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();
            _deviceChannel = _connection.CreateModel();

           // _channel.ExchangeDeclare("sensorQueue.exchange", ExchangeType.Topic);
            _channel.QueueDeclare("usersQueue", durable: true, exclusive: false);
            //_channel.QueueBind("sensorQueue", "sensorQueue.exchange", "", null);
            _deviceChannel.QueueDeclare("deviceQueue", durable: true, exclusive: false);
           // _deviceChannel.ExchangeDeclare("deviceQueue.exchange", ExchangeType.Topic);
           // _deviceChannel.QueueBind("deviceQueue", "deviceQueue.exchange", "", null);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            bool first = true;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<DeviceAPIDbContext>();
                
                List<UserDeviceRequest> userDeviceRequests = new List<UserDeviceRequest>();

                var usersWithDevices = _dbContext.UserClones.ToList();
                var deviceInstances = _dbContext.DeviceInstances.ToList();
                var deviceTypes = _dbContext.DeviceTypes.ToList();
                

                foreach (var user in usersWithDevices)
                {
                    foreach (var deviceInstance in deviceInstances)
                    {

                        var userDeviceRequest = new UserDeviceRequest
                        {
                            ID_User = user.ID_User,
                            ID_Dev_Inst = deviceInstance.ID_Dev_Inst,
                            ID_Dev_Type = deviceInstance.ID_Dev_Type,
                            MaxValue = deviceTypes.Where(dt => dt.ID_Dev_Type == deviceInstance.ID_Dev_Type).First().MaxPower
                        };

                        userDeviceRequests.Add(userDeviceRequest);
                        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(userDeviceRequest);
                        var body = Encoding.UTF8.GetBytes(jsonString);

                        _deviceChannel.BasicPublish(exchange: "", routingKey: "deviceQueue", basicProperties: null, body: body);
                        Console.WriteLine($"Sent message: {body} from device Queue");
                       // HandleMessage($"Sent message {body} from deviceQueue");
                    }
                }
            }

            List<string> vals = new List<string>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                if (first && (message == "" || message == null))
                {
                    first = false;
                }
                else
                {
                    vals.Add(message);
                    Console.WriteLine($"UserClone message received: {message}");
                }
              
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<DeviceAPIDbContext>();

                        UserClone userRequest = new UserClone
                        {
                            ID_User = vals.Last(),
                            Username = vals.Last(),
                        };

                        // Add your logic to use 'context' (DbContext) here

                        context.UserClones.Add(userRequest);
                        context.SaveChanges();
                    }
                
                HandleMessage(message + "from UserClone");

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: "usersQueue", autoAck: false, consumer: consumer);
            
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("usersQueue", false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            // we just print this message   
            _logger.LogInformation($"Service received {content}");
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
      
    }
}
