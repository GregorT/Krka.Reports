using System.Text.Json;
using Krka.Reports.ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
using NetMQ;
using NetMQ.Sockets;

namespace Krka.Reports.ApiGateway.Controllers;

[ApiController]
[Route("sinh")]
public class SinhController : ControllerBase, IDisposable
{
    #region Ctor

    private readonly string _address = "eventhub";
    //private readonly string _address = "127.0.0.1";

    public SinhController()
    {
        _publisher = new PublisherSocket();
        _publisher.Connect($"tcp://{_address}:11000");
        _subscriber = new SubscriberSocket();
        _subscriber.Connect($"tcp://{_address}:12000");
    }

    private readonly PublisherSocket _publisher;
    private readonly SubscriberSocket _subscriber;

    #endregion

    #region Public

    public void Dispose()
    {
        _publisher.Disconnect($"tcp://{_address}:11000");
        _publisher.Dispose();
        _subscriber.Disconnect($"tcp://{_address}:12000");
        _subscriber.Dispose();
    }

    //[HttpGet]
    //public async Task<IEnumerable<EnSifrant>?> Get([FromServices] HubService eventHub)
    //{
    //    Console.WriteLine("sending list");
    //    var channel = "Codelists.Demo";
    //    var key = Guid.NewGuid().ToString();
    //    Console.WriteLine("list sent");
    //    await eventHub.Subscribe($"{channel}.Response");
    //    var message = string.Empty;

    //    void Handler(object? sender, HubService.DataReceivedEventArgs e)
    //    {
    //        if (e.Channel != $"{channel}.Response") return;
    //        if (e.Key != key) return;
    //        message = e.Data;
    //    }

    //    eventHub.DataReceived += Handler;
    //    await eventHub.SendData($"{channel}.Request", key, "list");
    //    while (string.IsNullOrEmpty(message)) Thread.Sleep(30);
    //    await eventHub.Subscribe($"{channel}.Response");
    //    eventHub.DataReceived -= Handler;
    //    var result = JsonSerializer.Deserialize<List<EnSifrant>>(message);
    //    return result;
    //}

    [HttpPost]
    public async Task<IEnumerable<EnSifrant>?> Test(string command)
    {
        _publisher.Connect($"tcp://{_address}:11000");
        _subscriber.Connect($"tcp://{_address}:12000");
        try
        {
            var key = Guid.NewGuid().ToString();
            while (!_publisher.TrySignalOK())
            {
                Thread.Sleep(100);
            }
            _subscriber.Subscribe($"Codelists.Demo.Response.{key}");
            _publisher.SendMoreFrame("Codelists.Demo.Request").SendMoreFrame(key).SendFrame(command);
            var result = string.Empty;
            _subscriber.ReceiveReady += (_, args) =>
            {
                var topic = args.Socket.ReceiveFrameString();
                result = args.Socket.ReceiveFrameString();
                _subscriber.Unsubscribe($"Codelists.Demo.Response.{key}");
            };
            _subscriber.Poll(TimeSpan.FromSeconds(5));
            if (string.IsNullOrWhiteSpace(result)) return null;
            var data = JsonSerializer.Deserialize<List<EnSifrant>>(result);
            return data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion
}

public record EnSifrant(string Id, string Firstname, string Lastname, string Email);