using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MyServer.Abstraction;

namespace MyServer.Middlewares;

public class MetadataMiddleware : IMiddleware
{
    private readonly Repository _repository;

    public MetadataMiddleware()
    {
        _repository = Repository.Create();
    }

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
        
        Console.WriteLine("===== REQUEST =====");
        foreach (var l in requestLines)
            Console.WriteLine(l);

        var firstLine = requestLines[0]; // GET /hello HTTP/1.1
        var parts = firstLine.Split(' ');

        var verb = firstLine.Split(' ')[0];
        
        var method = parts[0];
        var path = parts[1];

        _repository.Body = body;
        _repository.Header = requestLines;
        _repository.Path = path;
        _repository.Method = method;
    }
}