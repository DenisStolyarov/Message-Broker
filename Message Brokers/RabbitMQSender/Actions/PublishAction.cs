using System.Text;
using RabbitMQ.Client;

namespace RabbitMQSender.Actions;

public static class PublishAction
{
    private const string ExchangeName = "publish";

    public static void Start(IModel channel)
    {
        channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Fanout);

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
                exchange: ExchangeName,
                routingKey: string.Empty,
                basicProperties: null,
                body: body);
        }
    }
}