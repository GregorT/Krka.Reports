using NetMQ;
using NetMQ.Sockets;
//var _address = "eventhub";
var _address = "127.0.0.1";

Console.WriteLine("EventHub starting...");
using var incomingSocket = new XSubscriberSocket();
incomingSocket.Bind($"tcp://{_address}:11000");
using var outgoingSocket = new XPublisherSocket();
outgoingSocket.Bind($"tcp://{_address}:12000");
var proxy = new Proxy(incomingSocket, outgoingSocket);
Console.WriteLine("EventHub started.");
proxy.Start();