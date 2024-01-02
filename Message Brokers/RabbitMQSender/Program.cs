using RabbitMQ.Client;
using RabbitMQSender.Actions;

var factory = new ConnectionFactory()
{
    HostName = "localhost",
};

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

TopicAction.Start(channel);

Console.ReadLine();