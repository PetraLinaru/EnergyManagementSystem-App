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
using Microsoft.AspNetCore.Connections;

public interface IRabbitMQProducerService
{
    public Task<Response> ProduceAsync(string ID);
}

public class RabbitMQProducerService : IRabbitMQProducerService { 

    public RabbitMQProducerService()
    {

    }

    public async Task<Response> ProduceAsync(string ID)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@rabbitmq2")
        };

        var connection = factory.CreateConnection();

        using var channel = connection.CreateModel();


        channel.QueueDeclare(queue: "usersQueue", durable: true, exclusive: false);

        List<string> vals = new List<string>();
        var body = Encoding.UTF8.GetBytes(ID);
        channel.BasicPublish(exchange: "", routingKey: "usersQueue", basicProperties: null, body: body);
        Console.WriteLine($"********************User message SENT: {ID}*********************");

        return new Response
        {
            IsSuccess = true,
            Message = "Sent"
        };
    }

}