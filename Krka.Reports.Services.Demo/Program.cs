using Krka.Reports.Services.Demo.Logic;
using NetMQ;
using NetMQ.Sockets;

var _address = "eventhub";
//var _address = "127.0.0.1";

Console.WriteLine("Subsrciber starting...");
var tasks = new List<Task>();
var topic = "Codelists.Demo.Request";

var pubSocket = new PublisherSocket();
pubSocket.Connect($"tcp://{_address}:11000");

var subSocket = new SubscriberSocket();
subSocket.Connect($"tcp://{_address}:12000");
subSocket.Options.ReceiveHighWatermark = 0;
subSocket.Subscribe(topic);
Console.WriteLine("Subscriber socket connected. waiting...");
while (true)
{
    var topicReceived = subSocket.ReceiveFrameString();
    var keyReceived = subSocket.ReceiveFrameString();
    var messageReceived = subSocket.ReceiveFrameString();
    var payloadReceived = subSocket.ReceiveFrameString();
    Console.WriteLine($"received: {keyReceived} - {messageReceived} - {payloadReceived}");
    if (messageReceived.Equals("list")) await new DemoLogic().SendList(pubSocket, keyReceived);
}

subSocket.Close();
subSocket.Dispose();