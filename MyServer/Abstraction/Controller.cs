using System.Text.Json;
using MyServer.Model.Abstraction;
namespace MyServer.Abstraction;

public abstract class Controller
{
    public ActionResult Ok<T>(T value = default)
    {
        if(value is string)
            return new ActionResult(value as string, "HTTP/1.1 200 OK "); 
        return new ActionResult(JsonSerializer.Serialize(value), "HTTP/1.1 200 OK ");
    }

    public ActionResult NotFound<T>(T value = default)
    {
        if (value is string)
            return new ActionResult(value as string, "HTTP/1.1 404 Not Found");
        return new ActionResult(JsonSerializer.Serialize(value), "HTTP/1.1 404 Not Found");
    } 
}