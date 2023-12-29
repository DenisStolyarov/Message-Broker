using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQReceiver.Actions;

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

        channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false);

        EventingBasicConsumer consumer = new(channel);

        consumer.Received += (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);

            Thread.Sleep(5000);

            Console.WriteLine($"[i]: Received - {message}");

            channel.BasicAck(
                ea.DeliveryTag,
                multiple: false);
        };

        channel.BasicConsume(
            queue: "worker",
            autoAck: false,
            consumer);
    }
}
