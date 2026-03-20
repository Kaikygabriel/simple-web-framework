using System.Net;
using System.Net.Sockets;
using MyServer.Application;

namespace MyServer.Builder;

public class WebBuilder
{
    private readonly int _port;

    public WebBuilder(int port)
    {
        _port = port;
    }

    public WebApplication Build()
    {
        var listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();
        return new WebApplication(listener,_port);
    }
}