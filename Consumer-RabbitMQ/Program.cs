using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Consumer_RabbitMQ.DTO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class Program
{
    static void Main()
    {
        var response = await lerLista();
    }

    public async Task<IActionResult> lerLista()
    {
        // Configuração da conexão com o RabbitMQ
        var factory = new ConnectionFactory
        {
            HostName = "localhost", // Substitua pelo endereço do seu servidor RabbitMQ
            UserName = "guest",
            Password = "guest",
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            string queueName = "queue-tjmg"; // Substitua pelo nome da sua fila

            // Certifique-se de que a fila já foi declarada previamente
            channel.QueueDeclare(queue: queueName,
                                                durable: true,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);

            // Configura o consumidor
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var queueDto = JsonConvert.DeserializeObject<QueueDTO>(message);

                Console.WriteLine($" [x] Received: Filiacao={queueDto.filiacao}, Nome={queueDto.nome}, CPF={queueDto.cpf}, Data de Nascimento={queueDto.data_nascimento}");
            };

            // Inicia o consumidor na fila específica
            channel.BasicConsume(queue: queueName,
                                                autoAck: true,
                                                                                consumer: consumer);

            Console.WriteLine($"Aguardando mensagens da fila '{queueName}'. Pressione [enter] para sair.");
            Console.ReadLine();

            return Ok();
        }
    }   
}
