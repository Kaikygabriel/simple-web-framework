using System.Net.Sockets;
using System.Text;
using MyServer.Abstraction;
using MyServer.Attributes.Methods;
using MyServer.Context;
using MyServer.Model.Abstraction;

namespace MyServer.Middlewares;

public class ControllerMiddleware : IMiddleware
{
    public async Task Execute(TcpClient client)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);

        var requestLines = new List<string>();
        string line;

        while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
            requestLines.Add(line);
        
        if (requestLines.Count == 0)
            return;

        Console.WriteLine("===== REQUEST =====");
        foreach (var l in requestLines)
            Console.WriteLine(l);

        // ✅ Parse da primeira linha
        var firstLine = requestLines[0]; // GET /hello HTTP/1.1
        var parts = firstLine.Split(' ');

        var verb = firstLine.Split(' ')[0];
        
        var method = parts[0];
        var path = parts[1];

        ActionResult? result = HttpGetAttribute.ExecuteAction(path);

        if (result is null)
            result = new("404 Not Found", "HTTP/1.1 404 Not Found");
        
        var response = Response.Create(result);

        var responseBytes = Encoding.UTF8.GetBytes(response);
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
        await stream.FlushAsync();
    }
}