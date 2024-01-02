using System.Text;
using RabbitMQ.Client;

namespace RabbitMQSender.Actions;

public static class RoutingAction
{
    private const string ExchangeName = "routing";

    public static void Start(IModel channel)
    {
        channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Direct);

        for (int i = 0; i < 10; i++)
        {
            string routingKey = i % 2 == 0
                ? "info"
                : "error";
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