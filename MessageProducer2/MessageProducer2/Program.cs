using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.VisualBasic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

class Program
{
    
    static void Main()
    {
        Console.Write("Enter file index (1/2/3): ");
        if (!int.TryParse(Console.ReadLine(), out int fileIndex) || fileIndex < 1 || fileIndex > 3)
        {
            Console.WriteLine("Invalid file index. Choose a value between 1 and 3.");
            return;
        }

        var configFile = GetConfigFilePath(fileIndex);
        string[] lines = File.ReadAllLines(configFile);

        if (lines.Length < 2)
        {
            Console.WriteLine("Invalid configuration file format.");
            return;
        }
        string idUser = lines[0].Trim();
        int idDevInst;
        if (!int.TryParse(lines[1].Trim(), out idDevInst))
        {
            Console.WriteLine("Invalid configuration file format. ID_Dev_Inst must be an integer.");
            return;
        }
        Console.WriteLine("Sending data to user: "+idUser+" with id_dev_inst "+idDevInst);
        var factory = new ConnectionFactory
        {
           Uri = new Uri("amqp://guest:guest@localhost:5673")
        };

        var connection = factory.CreateConnection();
        
        var file1="/Users/petralinaru/Desktop/DS2023_30644_Linaru_Petra_1/MessageProducer2/MessageProducer2/config1.txt";
        var file2="/Users/petralinaru/Desktop/DS2023_30644_Linaru_Petra_1/MessageProducer2/MessageProducer2/config2.txt";
        var file3="/Users/petralinaru/Desktop/DS2023_30644_Linaru_Petra_1/MessageProducer2/MessageProducer2/config3.txt";
        using
        var channel = connection.CreateModel();
        
        channel.QueueDeclare(queue: "sensorQueue2", durable: true, exclusive: false);

        
            Debug.WriteLine("trimitem");
            Console.WriteLine("Trimitem");

            string filepath="/Users/petralinaru/Desktop/DS2023_30644_Linaru_Petra_1/MessageProducer2/MessageProducer2/MessageProducer.csv";
            using (StreamReader streamReader = new StreamReader(filepath))
            using (CsvReader csvReader = new CsvReader(streamReader, new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        }))
        {
            var records = csvReader.GetRecords<float>();

            DateTime currentTime = DateTime.Now;
            foreach (var sensorData in records)
            {
                
                SensorRequest request=new SensorRequest{
                    ID_Dev_Inst=idDevInst,
                    ID_User=idUser,
                    Value=sensorData,
                    Time = currentTime.ToString("MM/dd/yyyy h:mm:ss tt")
                };
                Thread.Sleep(2000);

                string jsonString = JsonConvert.SerializeObject(request);
                var body = Encoding.UTF8.GetBytes(jsonString);
               
                channel.BasicPublish(exchange: "", routingKey: "sensorQueue2", basicProperties: null, body: body);
                Console.WriteLine($"Sent message: {sensorData}");

                currentTime = currentTime.AddMinutes(20);
            }
        }
    }

    private static string GetConfigFilePath(int fileIndex)
    {
        if(fileIndex==1){
            return "/Users/petralinaru/Desktop/DS2023_30644_Linaru_Petra_1/MessageProducer2/MessageProducer2/config1.txt";
        }
        if(fileIndex==2){
            return "/Users/petralinaru/Desktop/DS2023_30644_Linaru_Petra_1/MessageProducer2/MessageProducer2/config2.txt";
        }
        return "/Users/petralinaru/Desktop/DS2023_30644_Linaru_Petra_1/MessageProducer2/MessageProducer2/config13.txt";
    }

}
  


