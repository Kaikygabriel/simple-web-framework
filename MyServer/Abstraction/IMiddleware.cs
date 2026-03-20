using System.Net.Sockets;

namespace MyServer.Abstraction;

public interface IMiddleware
{
    Task Execute(TcpClient client);
}