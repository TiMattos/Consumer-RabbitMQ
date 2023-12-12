using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Worker_RabbitMQ.DTO;

namespace Worker_RabbitMQ
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await publishQueue();
                    await ReadQueue();
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        public static async Task ReadQueue()
        {
           
            var factory = new ConnectionFactory
            {
                HostName = "localhost", 
                UserName = "guest",
                Password = "guest",
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                string queueName = "queue-tjmg"; 

                channel.QueueDeclare(queue: queueName,
                                                    durable: true,
                                                    exclusive: false,
                                                    autoDelete: false,
                                                    arguments: null);


                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var queueDto = JsonConvert.DeserializeObject<QueueDTO>(message);
                    Console.WriteLine($"Address: {queueDto.address}\nName: {queueDto.name}\nCPF: {queueDto.cpf}\nBirth Date: {queueDto.birthDate}");
                
                };
            channel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: consumer);
            Console.WriteLine($"Aguardando mensagens da fila '{queueName}'. Pressione [enter] para sair.");
                Console.ReadLine();

               
            }
        }


        public static async Task publishQueue()
        {
            
            var factory = new ConnectionFactory
            {
                HostName = "localhost", 
                UserName = "guest",
                Password = "guest",
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                string queueName = "queue-tjmg";

                
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);


                var person = new QueueDTO
                {
                    address = "123 Main Street",
                    name = "John Doe",
                    cpf = "123456789",
                    birthDate = "1990-01-01"
                };


                var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(person);

                
                var body = Encoding.UTF8.GetBytes(messageJson);

                
                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine($" [x] Mensagem publicada na fila '{queueName}'");
            }

            //Console.ReadLine(); 
        }


    }
}
