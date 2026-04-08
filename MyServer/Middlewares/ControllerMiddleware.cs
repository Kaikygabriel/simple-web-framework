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
    
    private readonly Repository _repository;

    public ControllerMiddleware()
    {
        _repository = Repository.Create();
    }

    
    public async Task Execute(NetworkStream stream)
    {
        ActionResult? result = GetMethod(_repository.Method,_repository.Path,_repository.Body,_repository.Header);

        if (result is null)
            result = new("404 Not Found", "HTTP/1.1 404 Not Found");
        
        var response = Response.Create(result);

        var responseBytes = Encoding.UTF8.GetBytes(response);
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
        await stream.FlushAsync();
    }

    private ActionResult? GetMethod(string method,string path,string body,List<string> linesHeader)
    {
        if (method.Equals("Get", StringComparison.CurrentCultureIgnoreCase))
            return HttpGetAttribute.ExecuteAction(path,linesHeader);
        if (method.Equals("Post", StringComparison.CurrentCultureIgnoreCase))
            return HttpPostAttribute.ExecuteAction(path,linesHeader,body);
        if (method.Equals("Put", StringComparison.CurrentCultureIgnoreCase))
            return HttpPutAttribute.ExecuteAction(path,linesHeader,body);
        if (method.Equals("Delete", StringComparison.CurrentCultureIgnoreCase))
            return HttpDeleteAttribute.ExecuteAction(path,linesHeader,body);

        return null;
    }
}