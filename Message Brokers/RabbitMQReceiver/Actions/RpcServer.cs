using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQReceiver.Actions;

public static class RpcServer
{
    private const string QueueName = "rpc";

    public static void Start(IModel channel)
    {
        channel.QueueDeclare(
            queue: QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false);

        EventingBasicConsumer consumer = new(channel);

        consumer.Received += HandleRequest;

        channel.BasicConsume(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);

        void HandleRequest(object? model, BasicDeliverEventArgs ea)
        {
            byte[] bodyIn = ea.Body.ToArray();
            string request = Encoding.UTF8.GetString(bodyIn);

            Console.WriteLine($"[i]: Received - {request}");

            string response = $"Handled: {request}";
            byte[] bodyOut = Encoding.UTF8.GetBytes(response);

            IBasicProperties replayProps = channel.CreateBasicProperties();
            replayProps.CorrelationId = ea.BasicProperties.CorrelationId;

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: ea.BasicProperties.ReplyTo,
                basicProperties: replayProps,
                body: bodyOut);

            channel.BasicAck(
                deliveryTag: ea.DeliveryTag,
                multiple: false);
        }
    }
}