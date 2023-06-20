using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Krka.Reports.Services.MinApi;

public class RabbitConnector
{
    #region Public

    public Task<T?> Receive<T>(IModel channel, string topic, string key)
    {
        var timeout = DateTime.Now.AddSeconds(5);
        channel.QueueDeclare($"{topic}.{key}", exclusive: false);
        var consumer = new EventingBasicConsumer(channel);
        var data = default(T);
        var finished = false;
        consumer.Received += (sender, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            data = JsonSerializer.Deserialize<T>(message);
            channel.BasicAck(args.DeliveryTag, false);
            finished = true;
        };
        channel.BasicConsume($"{topic}.{key}", false, consumer);
        
        while (!finished) Task.Run(() => Thread.Sleep(10));
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