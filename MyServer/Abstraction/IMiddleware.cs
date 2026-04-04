using System.Net.Sockets;
using System.Threading.Tasks;

namespace MyServer.Abstraction;

public interface IMiddleware
{
    Task Execute(TcpClient client);
}