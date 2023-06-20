using System.Text;
using System.Text.Json;
using Krka.Reports.Services.Demo.Logic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var rabbithost = Environment.GetEnvironmentVariable("RABBIT");
if (rabbithost == null || string.IsNullOrWhiteSpace(rabbithost))
{
    Console.WriteLine("Environment variable RABBIT is missing or not set");
    return;
}

Console.WriteLine("Subsrciber starting...");
var factory = new ConnectionFactory {HostName = rabbithost, UserName = "svcdemo", Password = "demo123"};
var connection = factory.CreateConnection();
var channel = connection.CreateModel();
var pubchannel = connection.CreateModel();
channel.QueueDeclare("Codelists.Demo.Request", exclusive: false);
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"received message={message}");
    channel.BasicAck(eventArgs.DeliveryTag, false);
    var data = JsonSerializer.Deserialize<Data>(message);
    if (data != null)
    {
        var key = data.Key;
        if (data.Command.Equals("list", StringComparison.InvariantCultureIgnoreCase)) Task.Run(async () => await new DemoLogic().SendList(pubchannel, key));
    }
};
channel.BasicConsume(queue: "Codelists.Demo.Request", autoAck: false, consumer: consumer);
Console.WriteLine("Subsrciber started.");
var manualResetEvent = new ManualResetEvent(false);
manualResetEvent.WaitOne();

public record Data(string Key, string Command, string? Payload);
public record Config(string RabbitHost);