using System.Net.Sockets;
using System.Reflection;
using System.Text;
using MyServer.Abstraction;
using MyServer.Attributes;
using MyServer.Attributes.Methods;
using MyServer.Attributes.Parameters;
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

        var queryString = "";
        if (path.Contains("?"))
        {
            var split = path.Split("?");
            path = split[0];         // "/hello"
            queryString = split[1];  // "name=kaiky&age=18"
        }
        
        var queryParams = new Dictionary<string, string>();
        foreach (var a in queryString.Split('&'))
        {
            var parameter = a.Split('=');
            if (parameter.Length == 2)
            {
                queryParams[parameter[0]] = parameter[1];
            }
        }
        
        string responseBody = "";
        string statusLine = "";

        foreach (var metodo in metodos)
        {
            var atributo = metodo.GetCustomAttribute<HttpGetAttribute>();
            if ( atributo is not null&&path.Equals("/" + atributo.EndPointName))
            {
                var instancia = Activator.CreateInstance(metodo.DeclaringType!);
                var parametros = metodo.GetParameters();

                ActionResult result;
                if (parametros.Length > 0)
                {
                    var argumentos = new object?[parametros.Length];
                    for (var p =0; p < parametros.Length ; p++)
                    {
                        var parametro = parametros[p];
                        var fromQuery = parametro.GetCustomAttribute<FromQueryAttribute>();
                    
                        if (fromQuery != null)
                        {
                            var nome = fromQuery.Value ?? parametro.Name!;
                    
                            if (queryParams.TryGetValue(nome, out string valor))
                                argumentos[p] = Convert.ChangeType(valor, parametro.ParameterType);
                            else
                                argumentos[p] = null;
                        }
                    } 
                    result = (ActionResult)metodo.Invoke(instancia,argumentos)!;
                }
                else
                 result = (ActionResult)metodo.Invoke(instancia,null)!;

              
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

        var responseBytes = Encoding.UTF8.GetBytes(response);
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
        await stream.FlushAsync();
    }
}