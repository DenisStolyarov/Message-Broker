using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQReceiver.Actions;

public static class SubscribeAction
{
    private const string ExchangeName = "publish";

    public static void Start(IModel channel)
    {
        channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Fanout);

        string queueName = channel.QueueDeclare().QueueName;

        channel.QueueBind(
            queue: queueName,
            exchange: ExchangeName,
            routingKey: string.Empty);

        EventingBasicConsumer consumer = new(channel);

        consumer.Received += (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"[i]: Received - {message}");
        };

        channel.BasicConsume(
            queue: queueName,
            autoAck: true,
            consumer);
    }
}
