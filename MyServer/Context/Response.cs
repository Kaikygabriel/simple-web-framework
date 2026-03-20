using System.Text;

namespace MyServer.Context;

public static class Response
{
    public static string Create(string status, string responseBody)
    {
        return 
            $"{status}\r\n" +
            "Content-Type: text/plain; charset=utf-8\r\n" +
            $"Content-Length: {Encoding.UTF8.GetByteCount(responseBody)}\r\n" +
            "\r\n" +
            responseBody;
    }
}