using NetMQ;
using NetMQ.Sockets;

namespace Krka.Reports.ApiGateway.Services;

public class HubService : IHostedService
{
    #region Ctor

    public class DataReceivedEventArgs : EventArgs
    {
        #region Properties

        public string? Channel { get; set; }

        public string? Data { get; set; }
        public string? Key { get; set; }

        #endregion
    }

    private readonly string _addressPublisher = "tcp://eventhub:11000";
    private readonly string _addressSubscriber = "tcp://eventhub:12000";
    public PublisherSocket Publisher = new();
    public SubscriberSocket Subscriber = new();

    #endregion

    #region Public

    public event EventHandler<DataReceivedEventArgs>? DataReceived;

    public Task SendData(string channel, string key, string message)
    {
        Publisher.SendMoreFrame(channel).SendMoreFrame(key).SendFrame(message);
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Publisher = new PublisherSocket();
        Publisher.Connect(_addressPublisher);
        Subscriber = new SubscriberSocket();
        Subscriber.Connect(_addressSubscriber);
        Subscribe("Quiet");
        Task.Run(() => ReceiveData(cancellationToken), cancellationToken);
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

    public Task Subscribe(string channel)
    {
        Subscriber.Subscribe(channel);
        return Task.CompletedTask;
    }

    public Task UnSubscribe(string channel)
    {
        Subscriber.Unsubscribe(channel);
        return Task.CompletedTask;
    }

    #endregion

    #region Private

    private Task ReceiveData(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested) return Task.FromResult(string.Empty);
            var channelReceived = Subscriber.ReceiveFrameString();
            var keyReceived = Subscriber.ReceiveFrameString();
            var messageReceived = Subscriber.ReceiveFrameString();
            OnDataReceived(this, channelReceived, keyReceived, messageReceived);
        }
    }

    #endregion


    protected virtual void OnDataReceived(object sender, string channel, string key, string data)
    {
        DataReceived?.Invoke(sender, new DataReceivedEventArgs {Channel = channel, Key = key, Data = data});
    }
}