using System.Text.Json;
using MyServer.Model;

namespace MyServer.Abstraction;

public abstract class Controller
{
    public ActionResult Ok<T>(T value = default)
    {
        if(value is string)
            return new ActionResult(value as string, "HTTP/1.1 200 OK"); 
        return new ActionResult(JsonSerializer.Serialize(value), "HTTP/1.1 200 OK");
    } 
}