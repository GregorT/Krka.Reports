using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Krka.Reports.Services.Demo;

public class RabbitConnector
{
    #region Public

    public Task<T?> Receive<T>(IModel channel, string topic, string key)
    {
        var timeout = DateTime.Now.AddSeconds(5);
        channel.QueueDeclare(topic, exclusive: false);
        var consumer = new EventingBasicConsumer(channel);
        var data = default(T);
        var finished = false;
        consumer.Received += (sender, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            data = JsonSerializer.Deserialize<T>(message);
            finished = true;
            channel.BasicAck(args.DeliveryTag, true);
        };
        channel.BasicConsume(topic, true, consumer);
        while (!finished || DateTime.Now < timeout) Task.Run(() => Thread.Sleep(100));
        return Task.FromResult(data);
    }

    public Task Send<T>(IModel channel, string topic, T data)
    {
        channel.QueueDeclare(topic, exclusive: false);
        var message = JsonSerializer.Serialize(data);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(string.Empty, topic, null, body);
        return Task.CompletedTask;
    }

    #endregion
}