using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using MyServer.Abstraction;
using MyServer.Builder;
using MyServer.Middlewares;

namespace MyServer.Application;

public class WebApplication
{
    private List<IMiddleware> _middlewares = new List<IMiddleware>();
    private readonly TcpListener _client;
    public WebApplication(TcpListener client)
    {
        _client = client;
    }

    public static WebBuilder CreateBuilder(int port =5000)
        => new WebBuilder(port);
    
    public async Task Run()
    {
        while (true)
        {
            var client = await _client.AcceptTcpClientAsync();
            
            _ = Task.Run(async () =>
            {
                foreach (var middleware in _middlewares)
                    await middleware.Execute(client);

                client.Client.Shutdown(SocketShutdown.Both); 
                client.Close();
            });
        }
    }

    public void MapControllers()
        => _middlewares.Add(new ControllerMiddleware());
}