using Krka.Reports.Services.MinApi;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
var rabbithost = Environment.GetEnvironmentVariable("RABBIT");
if (rabbithost == null || string.IsNullOrWhiteSpace(rabbithost))
{
    Console.WriteLine("Environment variable RABBIT is missing or not set");
    return;
}

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "Krka reporting minimal api gateway";
    document.Description = "";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOpenApi();
app.UseSwaggerUi3();

app.UseHttpsRedirection();

app.MapGet("/sinh", async () =>
    {
        try
        {
            var factory = new ConnectionFactory { HostName = rabbithost, UserName = "minapi", Password = "demo123" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var key = Guid.NewGuid().ToString();
            var rc = new RabbitConnector();
            var recChannel = connection.CreateModel();
            await rc.Send(channel, "Codelists.Demo.Request", new Data(key, "list", null));
            var data =await rc.Receive<List<EnSifrant>>(recChannel, "Codelists.Demo.Response", key); ;
            connection.Close();
            return data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    })
    .WithName("TestMe")
    .WithOpenApi();

app.Run();

public record EnSifrant(string Id, string Firstname, string Lastname, string Email);
public record Data(string Key, string Command, string? Payload);