using NetMQ;
using NetMQ.Sockets;

namespace Krka.Reports.ApiGateway.Services;

public class EventHubSvc
{
    #region Ctor

    private readonly string _addressPublisher = "tcp://eventhub:11000";
    private readonly string _addressSubscriber = "tcp://eventhub:12000";
    private PublisherSocket _publisher = new();
    private SubscriberSocket _subscriber = new();

    #endregion

    #region Public

    public async Task<string> ReceiveData(string channel, string key, CancellationToken cancellationToken = default)
    {
        await StartSubscriber();
        _subscriber.Subscribe(channel);
        while (true)
        {
            if (cancellationToken.IsCancellationRequested) return string.Empty;
            var topicReceived = _subscriber.ReceiveFrameString();
            var keyReceived = _subscriber.ReceiveFrameString();
            if (!keyReceived.Equals(key)) continue;
            var messageReceived = _subscriber.ReceiveFrameString();
            _subscriber.Unsubscribe(channel);
            await StopSubsciber(cancellationToken);
            return messageReceived;
        }
    }

    public async Task<string> SendAndReceive(string channel, string key, string message, CancellationToken cancellation = default)
    {
        await SendData($"{channel}.Request", key, message);
        var data = await ReceiveData($"{channel}.Response", key, cancellation);
        return data;
    }

    public async Task SendData(string channel, string key, string message)
    {
        await StartPublisher();
        _publisher.SendMoreFrame(channel).SendMoreFrame(key).SendFrame(message);
        await StopPublisher();
    }

    #endregion

    #region Private

    private Task StartPublisher()
    {
        _publisher = new PublisherSocket();
        _publisher.Connect(_addressPublisher);
        return Task.CompletedTask;
    }

    private Task StartSubscriber()
    {
        _subscriber = new SubscriberSocket();
        _subscriber.Connect(_addressSubscriber);
        return Task.CompletedTask;
    }

    private Task StopPublisher(CancellationToken cancellation = default)
    {
        _publisher.Disconnect(_addressPublisher);
        _publisher.Dispose();
        return Task.CompletedTask;
    }

    private Task StopSubsciber(CancellationToken cancellationToken = default)
    {
        _subscriber.Disconnect(_addressSubscriber);
        _subscriber.Close();
        return Task.CompletedTask;
    }

    #endregion
}