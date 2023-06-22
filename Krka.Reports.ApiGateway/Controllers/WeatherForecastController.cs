using System.Text.Json;
using Krka.Reports.ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
using NetMQ.Sockets;

namespace Krka.Reports.ApiGateway.Controllers;

[ApiController]
[Route("sinh")]
public class SinhController : ControllerBase
{
    #region Ctor


    #endregion

    #region Public

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
    public async Task<IEnumerable<EnSifrant>?> Test([FromServices] HubService hub, string command)
    {
        try
        {
            var key = Guid.NewGuid().ToString();

            var result = await hub.SendAndReceive("Codelists.Demo", key, command, null, CancellationToken.None);
            if (result?.Data == null) return null;
            var data = JsonSerializer.Deserialize<List<EnSifrant>>(result.Data);
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