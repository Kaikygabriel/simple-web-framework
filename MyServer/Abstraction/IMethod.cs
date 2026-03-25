using MyServer.Model.Abstraction;

namespace MyServer.Abstraction;

public interface IMethod
{
    static abstract ActionResult? ExecuteAction(string path);
}