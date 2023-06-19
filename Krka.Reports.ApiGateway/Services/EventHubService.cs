using NetMQ;
using NetMQ.Sockets;

namespace Krka.Reports.ApiGateway.Services;

public class EventHubService : IHostedService
{
    #region Ctor

    private readonly string _addressPublisher = "tcp://eventhub:11000";
    private readonly string _addressSubscriber = "tcp://eventhub:12000";
    public PublisherSocket Publisher = new();
    public SubscriberSocket Subscriber = new();

    #endregion

    #region Public

    public async Task<string> SendAndReceive(string channel, string key, string message, CancellationToken cancellation = default)
    {
        await SendData($"{channel}.Request", key, message);
        var data = await ReceiveData($"{channel}.Response", key, cancellation);
        return data;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Publisher = new PublisherSocket();
        Publisher.Connect(_addressPublisher);
        Subscriber = new SubscriberSocket();
        Subscriber.Connect(_addressSubscriber);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Publisher.Disconnect(_addressPublisher);
        Publisher.Dispose();
        Subscriber.Disconnect(_addressSubscriber);
        Subscriber.Close();
        return Task.CompletedTask;
    }

    #endregion

    #region Private

    private Task<string> ReceiveData(string channel, string key, CancellationToken cancellationToken = default)
    {
        Subscriber.Subscribe(channel);
        while (true)
        {
            if (cancellationToken.IsCancellationRequested) return Task.FromResult(string.Empty);
            var topicReceived = Subscriber.ReceiveFrameString();
            var keyReceived = Subscriber.ReceiveFrameString();
            if (!keyReceived.Equals(key)) continue;
            var messageReceived = Subscriber.ReceiveFrameString();
            Subscriber.Unsubscribe(channel);
            return Task.FromResult(messageReceived);
        }
    }

    private Task SendData(string channel, string key, string message)
    {
        Publisher.SendMoreFrame(channel).SendMoreFrame(key).SendFrame(message);
        return Task.CompletedTask;
    }

    #endregion
}