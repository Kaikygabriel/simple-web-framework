using System.Net.Sockets;
using System.Reflection;
using System.Text;
using MyServer.Abstraction;
using MyServer.Attributes;
using MyServer.Context;
using MyServer.Model.Abstraction;

namespace MyServer.Middlewares;

public class ControllerMiddleware : IMiddleware
{
    public async Task Execute(TcpClient client)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var metodos = assembly.GetTypes()
            .SelectMany(t => t.GetMethods())
            .Where(m => m.GetCustomAttribute<HttpGetAttribute>() != null);
        
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

        var method = parts[0];
        var path = parts[1];

        string responseBody = "";
        string statusLine = "";

        foreach (var metodo in metodos)
        {
            var atributo = metodo.GetCustomAttribute<HttpGetAttribute>();
            if ( atributo is not null&&path.Equals("/" + atributo.EndPointName))
            {
                var instancia = Activator.CreateInstance(metodo.DeclaringType!);

                var result = (ActionResult)metodo.Invoke(instancia,null)!;
                responseBody = result.ResponseBody;
                statusLine = result.StatusLine;
            }
        }

        if (string.IsNullOrEmpty(responseBody) && string.IsNullOrEmpty(statusLine))
        {
            responseBody = "404 Not Found";
            statusLine = "HTTP/1.1 404 Not Found";
        }

        var response = Response.Create(statusLine, responseBody);

        var writer = new StreamWriter(stream, Encoding.UTF8);
        await writer.WriteAsync(response);
        await writer.FlushAsync();
    }
}