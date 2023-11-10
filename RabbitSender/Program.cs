using RabbitMQ.Client;
using System.Text;

// configure connection
ConnectionFactory connectionFactory = new();
connectionFactory.Uri = new Uri("amqp://guest:guest@localhost:5672");
connectionFactory.ClientProvidedName = "Rabbit Sender App";

IConnection conn = connectionFactory.CreateConnection();
IModel channel = conn.CreateModel();

// configure exchange, queue, binding
string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueBind(queueName, exchangeName, routingKey, null);

// publish single message
// byte[] messageBodyBytes = Encoding.UTF8.GetBytes("Hello RabbitMQ Again");
// channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

for (int i = 1; i <= 60; i++)
{
    Console.WriteLine($"sending message {i}");

    byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"messsage #{i}");
    channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

    Thread.Sleep(1000);
}

channel.Close();
conn.Close();