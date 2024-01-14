using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQSender.Actions
{
    public static class RpcClient
    {

        public static async Task Start(IModel channel)
        {
            int count = 10;
            List<Task<string>> tasks = new(count);
            Client client = new(channel);

            for (int i = 0; i < count; i++)
            {
                string message = $"Data: {i}";

                tasks.Add(client.CallAsync(message));
            }

            while (tasks.Count is not 0)
            {
                Task<string> completedTask = await Task.WhenAny(tasks);

                Console.WriteLine($"[i]: Received: {completedTask.Result}");

                tasks.Remove(completedTask);
            }
        }

        private sealed class Client
        {
            private const string QueueName = "rpc";

            private readonly IModel _channel;
            private readonly string _replayQueueName;
            private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper = new();

            public Client(IModel channel)
            {
                _channel = channel;
                _replayQueueName = _channel.QueueDeclare().QueueName;

                EventingBasicConsumer consumer = new(_channel);

                consumer.Received += HandleResponse;

                channel.BasicConsume(
                    queue: _replayQueueName,
                    autoAck: true,
                    consumer: consumer);
            }

            public Task<string> CallAsync(string message, CancellationToken ct = default)
            {
                TaskCompletionSource<string> tcs = new();
                string correlationId = Guid.NewGuid().ToString();

                _callbackMapper.TryAdd(correlationId, tcs);

                IBasicProperties properties = _channel.CreateBasicProperties();

                properties.CorrelationId = correlationId;
                properties.ReplyTo = _replayQueueName;

                byte[] body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: QueueName,
                    basicProperties: properties,
                    body: body);

                ct.Register(() => _callbackMapper.TryRemove(correlationId, out _));

                return tcs.Task;
            }

            private void HandleResponse(object? model, BasicDeliverEventArgs ea)
            {
                if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var taskCompletionSource))
                {
                    return;
                }

                byte[] bodyIn = ea.Body.ToArray();
                string response = Encoding.UTF8.GetString(bodyIn);

                taskCompletionSource.TrySetResult(response);
            }
        }
    }
}
