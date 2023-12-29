﻿using RabbitMQ.Client;
using RabbitMQReceiver.Actions;

ConnectionFactory factory = new()
{
    HostName = "localhost",
};

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

WorkerAction.Start(channel);

Console.ReadLine();