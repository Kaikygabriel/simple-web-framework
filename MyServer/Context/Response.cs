using System.Text;
using MyServer.Model.Abstraction;

namespace MyServer.Context;

public static class Response
{
    public static string Create(ActionResult actionResult)
    {
        // return
        //     "HTTP/1.1 200 OK \r\n" +
        //     "Content-Type: text/html; charset=UTF-8 \r\n" +
        //     "Content-Length: 138 \r\n" +
        //     "\r\n" +
        //     "<html> <body> <h1>Olá, Mundo!</h1> </body> </html>";
        //     
        return 
            $"{actionResult.StatusLine}\r\n" +
            "Content-Type: application/json; charset=utf-8\r\n" +
            $"Content-Length: {Encoding.UTF8.GetByteCount(actionResult.ResponseBody)}\r\n" +
            "Connection: close\r\n" +
            "\r\n" +
            actionResult.ResponseBody;
    }
}