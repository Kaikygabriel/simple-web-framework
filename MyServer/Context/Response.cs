using System.Text;
using MyServer.Model.Abstraction;

namespace MyServer.Context;

public static class Response
{
    public static string Create(ActionResult actionResult)
    {
        return 
            $"{actionResult.StatusLine}\r\n" +
            "Content-Type: text/plain; charset=utf-8\r\n" +
            $"Content-Length: {Encoding.UTF8.GetByteCount(actionResult.ResponseBody)}\r\n" +
            "Connection: close\r\n" +
            "\r\n" +
            actionResult.ResponseBody;
    }
}