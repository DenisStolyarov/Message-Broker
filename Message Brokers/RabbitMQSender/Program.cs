using RabbitMQ.Client;
using RabbitMQSender.Actions;

var factory = new ConnectionFactory()
{
    HostName = "localhost",
};

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

RoutingAction.Start(channel);

Console.ReadLine();