using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
        
        //body
        var contentLength = 0;

        foreach (var header in requestLines)
        {
            if (header.StartsWith("Content-Length", StringComparison.OrdinalIgnoreCase))
            {
                var value = header.Split(':')[1].Trim();
                contentLength = int.Parse(value);
            }
        }
        string body = string.Empty;

        if (contentLength > 0)
        {
            var buffer = new char[contentLength];
            await reader.ReadBlockAsync(buffer, 0, contentLength);
            body = new string(buffer);
        }
        Console.WriteLine("===== BODY =====");
        Console.WriteLine(body);
        
        //body
        Console.WriteLine("===== REQUEST =====");
        foreach (var l in requestLines)
            Console.WriteLine(l);

        // ✅ Parse da primeira linha
        var firstLine = requestLines[0]; // GET /hello HTTP/1.1
        var parts = firstLine.Split(' ');

        var verb = firstLine.Split(' ')[0];
        
        var method = parts[0];
        var path = parts[1];

        ActionResult? result = GetMethod(method,path,body);

        if (result is null)
            result = new("404 Not Found", "HTTP/1.1 404 Not Found");
        
        var response = Response.Create(result);

        var responseBytes = Encoding.UTF8.GetBytes(response);
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
        await stream.FlushAsync();
    }

    private ActionResult? GetMethod(string method,string path,string body)
    {
        if (method.Equals("Get", StringComparison.CurrentCultureIgnoreCase))
            return HttpGetAttribute.ExecuteAction(path);
        if (method.Equals("Post", StringComparison.CurrentCultureIgnoreCase))
            return HttpPostAttribute.ExecuteAction(path,body);
        if (method.Equals("Put", StringComparison.CurrentCultureIgnoreCase))
            return HttpPutAttribute.ExecuteAction(path,body);
        if (method.Equals("Delete", StringComparison.CurrentCultureIgnoreCase))
            return HttpDeleteAttribute.ExecuteAction(path,body);

        return null;
    }
}