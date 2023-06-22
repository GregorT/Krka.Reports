using NetMQ;
using NetMQ.Sockets;
using System.Security.Policy;
using System.ServiceModel.Channels;

namespace Krka.Reports.ApiGateway.Services;

public class HubService : IHostedService
{
    #region Ctor

    public class DataReceivedEventArgs : EventArgs
    {
        #region Properties

        public string Channel { get; set; } = string.Empty;

        public string? Data { get; set; }
        public string Key { get; set; } = string.Empty;

        #endregion
    }

    private readonly string _addressPublisher = "tcp://eventhub:11000";
    private readonly string _addressSubscriber = "tcp://eventhub:12000";
    public PublisherSocket Publisher = new();
    public SubscriberSocket Subscriber = new();

    #endregion

    #region Public

    public event EventHandler<DataReceivedEventArgs>? DataReceived;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Publisher = new PublisherSocket();
        Publisher.Connect(_addressPublisher);
        Subscriber = new SubscriberSocket();
        Subscriber.Connect(_addressSubscriber);
        Subscriber.Subscribe("quiet");
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

public record NetMqMessage(string Key, string? Data);