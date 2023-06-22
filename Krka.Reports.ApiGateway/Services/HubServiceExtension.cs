using NetMQ;
using NetMQ.Sockets;

namespace Krka.Reports.ApiGateway.Services;

public static class HubServiceExtension
{
    #region Public

    public static async Task<NetMqMessage?> SendAndReceive(this HubService hub, string channel, string key, string command, string? payload, CancellationToken cancellation = default)
    {
        NetMqMessage? result = null;
        var subChannel = $"{channel}.Response.{key}";

        void HubDataReceived(object? sender, HubService.DataReceivedEventArgs e)
        {
            if (e.Channel != subChannel) return;
            result = new NetMqMessage(e.Key, e.Data);
        }

        hub.Subscriber.Subscribe(subChannel);
        var startTime = DateTime.Now;
        hub.DataReceived += HubDataReceived;
        await hub.Publisher.SendData($"{channel}.Request", key, command, payload);
        while (result == null && (DateTime.Now - startTime).TotalSeconds < 15) Thread.Sleep(100);
        hub.DataReceived -= HubDataReceived;
        hub.Subscriber.Unsubscribe(subChannel);
        return result;
    }

    #endregion

    #region Private

    private static Task SendData(this PublisherSocket publisher, string channel, string key, string message, string? payload)
    {
        payload ??= string.Empty;
        publisher.SendMoreFrame(channel).SendMoreFrame(key).SendMoreFrame(message).SendFrame(payload);
        return Task.CompletedTask;
    }

    #endregion
}