using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using emsbackend.Shared;
using emsbackend.Data;
using emsbackend.Models;
using System.Diagnostics;
using System.Threading.Channels;
using System.Text.Json;
using Newtonsoft.Json;
using System.Globalization;

public class RabbitMQConsumerService : BackgroundService
{


    private readonly ILogger _logger;
    private IConnection _connection;
    private IModel _channel;
    private IModel _deviceChannel;
    private readonly IServiceScopeFactory _serviceScopeFactory;



    public RabbitMQConsumerService(ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = loggerFactory.CreateLogger<RabbitMQConsumerService>();
        _serviceScopeFactory = serviceScopeFactory;
        InitRabbitMQ();
    }


    private void InitRabbitMQ()
    {
        var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@rabbitmq2") };




        // create connection  
        _connection = factory.CreateConnection();


      
        // create channel  
        _channel = _connection.CreateModel();
        //create secondary channel
        _deviceChannel = _connection.CreateModel();
       
        _channel.QueueDeclare("sensorQueue2", durable: true, exclusive: false);
      
        _deviceChannel.QueueDeclare("deviceQueue", durable: true, exclusive: false);

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        stoppingToken.ThrowIfCancellationRequested();
        bool first = true;
        List<Sensor> vals = new List<Sensor>();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Sensor receivedMessage = JsonConvert.DeserializeObject<Sensor>(message);

            if (first && (message == "" || message == null))
            {
                first = false;
            }
            else
            {
                vals.Add(receivedMessage);
                Console.WriteLine($"*****************Sensor message received: {receivedMessage}*****************");
                
            }

            using (var scope = _serviceScopeFactory.CreateScope())

            {
                var context = scope.ServiceProvider.GetRequiredService<SensorAPIDbContext>();
                if (context.UserDevices.Any(u => u.ID_User == vals.Last().ID_User || u.ID_Dev_Inst == vals.Last().ID_Dev_Inst))
                {
                    context.Sensors.Add(vals.Last());
                    context.SaveChanges();
                    Console.WriteLine($"*****************Sensor Added: {receivedMessage}*****************");
                }
                else
                    Console.WriteLine($"*****************Sensor not added: {receivedMessage}*****************");

            }
            HandleMessage(message);

            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: "sensorQueue2", autoAck: false, consumer: consumer);

        consumer.Shutdown += OnConsumerShutdown;
        consumer.Registered += OnConsumerRegistered;
        consumer.Unregistered += OnConsumerUnregistered;
        consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

        _channel.BasicConsume("sensorQueue2", false, consumer);



        //SECONDARY CHANNEL on device types


        List<UserDevice> userDevices = new List<UserDevice>();
        var consumer2 = new EventingBasicConsumer(_deviceChannel);
        consumer2.Received += (ch, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            UserDevice receivedMessage = JsonConvert.DeserializeObject<UserDevice>(message);
            Console.WriteLine(receivedMessage.ToString());

            if (first && (message == "" || message == null))
            {
                first = false;
            }
            else
            {
                userDevices.Add(receivedMessage);
                Console.WriteLine($"*****************From DeviceQueue message received: {receivedMessage}*****************");
            }


            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SensorAPIDbContext>();

                UserDevice user = userDevices.Last();
                context.UserDevices.Add(user);
                context.SaveChanges();

                Console.WriteLine($"*****************From DeviceQueue message added: {receivedMessage}*****************");

            }

            HandleMessage(message + " from deviceQueue");

            _deviceChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _deviceChannel.BasicConsume(queue: "deviceQueue", autoAck: false, consumer: consumer2);

        consumer2.Shutdown += OnConsumerShutdown;
        consumer2.Registered += OnConsumerRegistered;
        consumer2.Unregistered += OnConsumerUnregistered;
        consumer2.ConsumerCancelled += OnConsumerConsumerCancelled;

        _deviceChannel.BasicConsume("deviceQueue", false, consumer2);
     
        return Task.CompletedTask;
    }


    private void HandleMessage(string content)
    {
        // we just print this message   
        _logger.LogInformation($"*******Service received {content}*******");
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