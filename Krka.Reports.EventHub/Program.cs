using NetMQ;
using NetMQ.Sockets;

Console.WriteLine("EventHub starting...");
using var incomingSocket = new XSubscriberSocket("@tcp://0.0.0.0:11000");
using var outgoingSocket = new XPublisherSocket("@tcp://0.0.0.0:12000");
var proxy = new Proxy(incomingSocket, outgoingSocket);
Console.WriteLine("EventHub started.");
proxy.Start();