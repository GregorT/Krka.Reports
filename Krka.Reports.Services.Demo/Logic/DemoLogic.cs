using RabbitMQ.Client;

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

    public async Task SendList(IModel channel, string key)
    {
        var connector = new RabbitConnector();
        await connector.Send(channel, $"Codelists.Demo.Response.{key}", _sifrant);
        Console.WriteLine("sending list");
    }

    #endregion
}