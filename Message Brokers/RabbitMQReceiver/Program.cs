using RabbitMQ.Client;
using RabbitMQReceiver.Actions;

ConnectionFactory factory = new()
{
    HostName = "localhost",
};

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

RpcServer.Start(channel);

Console.ReadLine();