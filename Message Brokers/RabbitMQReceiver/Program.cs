using RabbitMQ.Client;
using RabbitMQReceiver.Actions;

ConnectionFactory factory = new()
{
    HostName = "localhost",
};

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

RoutingAction.Start(channel);

Console.ReadLine();