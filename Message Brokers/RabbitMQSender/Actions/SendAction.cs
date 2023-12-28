using System.Text;
using RabbitMQ.Client;

namespace RabbitMQSender.Actions
{
    public static class SendAction
    {
        public static void Start(IModel channel)
        {
            channel.QueueDeclare(
                queue: "common",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            for (int i = 0; i < 10; i++)
            {
                string message = $"Data - {i}";

                Send(channel, message);
                Console.WriteLine($"[i]: Sent - {message}");
            }

            static void Send(IModel channel, string message)
            {
                byte[] body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: "common",
                    basicProperties: null,
                    body: body);
            }
        }
    }
}
