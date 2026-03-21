using System.Text.Json;
using MyServer.Model.Abstraction;

namespace MyServer.Model;

public record ActionResult<T>(string ResponseBody, string StatusLine) 
{
    public static implicit operator ActionResult<T>(T type)
        => new(type is string srt
            ? srt
            : JsonSerializer.Serialize(type), "HTTP/1.1 200 OK");

    public static implicit operator Abstraction.ActionResult(ActionResult<T> result)
        => new(result.ResponseBody, result.StatusLine);
    
    public static implicit operator ActionResult<T>(ActionResult result)
        => new(result.ResponseBody, result.StatusLine);
}