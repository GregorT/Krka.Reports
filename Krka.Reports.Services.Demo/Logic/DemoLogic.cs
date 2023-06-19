using System.Text.Json;
using NetMQ;
using NetMQ.Sockets;

namespace Krka.Reports.Services.Demo.Logic;

internal class DemoLogic
{
    #region Ctor

    public record EnSifrant(string Id, string Firstname, string Lastname, string Email);

    private readonly List<EnSifrant> _sifrant = new()
    {
        new EnSifrant(Guid.NewGuid().ToString(), "Prvi", "Primer", "prvi.primer@email.com"),
        new EnSifrant(Guid.NewGuid().ToString(), "Drugi", "Primer", "drugi.primer@email.com"),
        new EnSifrant(Guid.NewGuid().ToString(), "Tretji", "Primer", "tretji.primer@email.com"),
        new EnSifrant(Guid.NewGuid().ToString(), "Cetrti", "Primer", "cetrti.primer@email.com"),
        new EnSifrant(Guid.NewGuid().ToString(), "Peti", "Primer", "peti.primer@email.com"),
        new EnSifrant(Guid.NewGuid().ToString(), "Sesti", "Primer", "sesti.primer@email.com"),
        new EnSifrant(Guid.NewGuid().ToString(), "Sedmi", "Primer", "sedmi.primer@email.com")
    };

    #endregion

    #region Public

    public async Task SendList(PublisherSocket pubSocket, string key)
    {
        var message = JsonSerializer.Serialize(_sifrant);
        Console.WriteLine("sending list");
        pubSocket.SendMoreFrame($"Codelists.Demo.Response.{key}").SendFrame(message);
    }

    public async Task SendSearch(PublisherSocket publisher, string key, string search)
    {
        var list = _sifrant.Where(p => p.Firstname.Contains(search, StringComparison.InvariantCultureIgnoreCase) || p.Lastname.Contains(search, StringComparison.InvariantCultureIgnoreCase)).ToList();
        var message = JsonSerializer.Serialize(_sifrant);
        Console.WriteLine("sending list");
        publisher.SendMoreFrame($"Codelists.Demo.Response.{key}").SendFrame(message);
    }

    #endregion
}