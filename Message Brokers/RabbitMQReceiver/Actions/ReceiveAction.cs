using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQReceiver.Actions
{
    public static class ReceiveAction
    {
        public static void Start(IModel channel)
        {
            channel.QueueDeclare(
                queue: "common",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            EventingBasicConsumer consumer = new(channel);

            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"[i]: Received - {message}");
            };

            channel.BasicConsume(
                queue: "common",
                autoAck: true,
                consumer);
        }
    }
}
