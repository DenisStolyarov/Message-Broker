using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQReceiver.Actions;

public static class RoutingAction
{
    private const string ExchangeName = "routing";

    public static void Start(IModel channel)
    {
        channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Direct);

        string queueName = channel.QueueDeclare().QueueName;

        foreach (string routingKey in GetRoutingKeys())
        {
            channel.QueueBind(
                queue: queueName,
                exchange: ExchangeName,
                routingKey: routingKey);
        }

        EventingBasicConsumer consumer = new(channel);

        consumer.Received += (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"[i]:{ea.RoutingKey}: Received - {message}");
        };

        channel.BasicConsume(
            queue: queueName,
            autoAck: true,
            consumer);

        static string[] GetRoutingKeys() =>
            Random.Shared.Next() % 2 == 0
                ? ["info"]
                : ["info", "error"];
    }


}

