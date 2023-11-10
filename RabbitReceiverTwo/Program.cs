using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

// configure connection
ConnectionFactory connectionFactory = new();
connectionFactory.Uri = new Uri("amqp://guest:guest@localhost:5672");
connectionFactory.ClientProvidedName = "Rabbit Receiver Two App";

IConnection conn = connectionFactory.CreateConnection();
IModel channel = conn.CreateModel();

// configure exchange, queue, binding
string queueName = "DemoQueue";
channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

// configure receiver/consumer
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
    Task.Delay(TimeSpan.FromSeconds(3)).Wait();

    var bodyBytes = args.Body.ToArray();

    string message = Encoding.UTF8.GetString(bodyBytes);

    Console.WriteLine($"message received: {message}");

    channel.BasicAck(args.DeliveryTag, multiple: false);
};

// shutdown consumer (cancel subscription)
string consumerTag = channel.BasicConsume(queueName, autoAck: false, consumer);

Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
conn.Close();