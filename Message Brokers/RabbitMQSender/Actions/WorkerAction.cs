using System.Text;
using RabbitMQ.Client;

namespace RabbitMQSender.Actions;

public static class WorkerAction
{
    public static void Start(IModel channel)
    {
        channel.QueueDeclare(
            queue: "worker",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        IBasicProperties properties = channel.CreateBasicProperties();
        
        properties.Persistent = true;

        for (int i = 0; i < 10; i++)
        {
            string message = $"Data - {i}";

            Send(channel, message, properties);

            Console.WriteLine($"[i]: Sent - {message}");
        }

        static void Send(IModel channel, string message, IBasicProperties? properties = null)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: "worker",
                basicProperties: properties,
                body: body);
        }
    }
}

