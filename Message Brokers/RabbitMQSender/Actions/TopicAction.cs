using System.Text;
using RabbitMQ.Client;

namespace RabbitMQSender.Actions;

public static class TopicAction
{
    private const string ExchangeName = "topic";

    public static void Start(IModel channel)
    {
        channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Topic);

        for (int i = 0; i < 10; i++)
        {
            string routingKey = i % 2 == 0
                ? "data.info"
                : "data.error";
            string message = $"Data - {i}";

            Send(channel, message, routingKey);

            Console.WriteLine($"[i]:{routingKey}: Sent - {message}");
        }

        static void Send(IModel channel, string message, string routingKey)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }
    }
}