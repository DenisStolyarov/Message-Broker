using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQReceiver.Actions;

public static class TopicAction
{
    private const string ExchangeName = "topic";

    public static void Start(IModel channel)
    {
        channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Topic);

        string queueName = channel.QueueDeclare().QueueName;
        string routingKey = GetRoutingKey();

        Console.WriteLine($"Receiver: {routingKey}");

        channel.QueueBind(
            queue: queueName,
            exchange: ExchangeName,
            routingKey: routingKey);

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

        static string GetRoutingKey() =>
             Random.Shared.Next(1, 12) switch
             {
                 < 3 => "data.info",
                 < 6 => "data.error",
                 < 9 => "data.*",
                 _ => "#",
             };
    }
}