using System.Text;
using Newtonsoft.Json;
using producer.Models;
using RabbitMQ.Client;
using Serilog;

namespace producer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://admin:admin@172.16.211.18/termidesk")
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "to_bpmn_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var cardRequest = new Card112ChangedRequest
            {
                GlobalId = Guid.NewGuid().ToString(),
                EmergencyCardId = "1234",
                Creator = "John Doe",
                // Дополните необходимыми полями
            };

            // Преобразование объекта в JSON
            var message = JsonConvert.SerializeObject(cardRequest);
            var body = Encoding.UTF8.GetBytes(message);

            // Публикация сообщения в RabbitMQ
            channel.BasicPublish(exchange: "",
                                 routingKey: "to_bpmn_queue",
                                 basicProperties: null,
                                 body: body);

            Log.Information("Message published to RabbitMQ to_bpmn_queue: {Message}", message);
        }
    }
}
