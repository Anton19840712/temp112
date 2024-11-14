using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "consumer";

            // Настройка Serilog для записи в файл
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            // Настройка RabbitMQ
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://admin:admin@172.16.211.18/termidesk")
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Создание обработчика сообщений
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Логирование полученного сообщения в файл
                Log.Information("Received message from RabbitMQ bpmn_queue: {Message}", message);

                // Логирование JSON-сообщения в файл
                Log.Information("Parsed JSON message:\n{ParsedMessage}", message);
            };

            // Начало потребления сообщений
            channel.BasicConsume(queue: "bpmn_queue",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
