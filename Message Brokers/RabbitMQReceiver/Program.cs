using RabbitMQ.Client;
using RabbitMQReceiver.Actions;

ConnectionFactory factory = new()
{
    HostName = "localhost",
};

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

TopicAction.Start(channel);

Console.ReadLine();