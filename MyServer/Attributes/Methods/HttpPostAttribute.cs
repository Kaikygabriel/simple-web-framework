using System.Reflection;
using MyServer.Attributes.Parameters;
using MyServer.Model.Abstraction;

namespace MyServer.Attributes.Methods;

[AttributeUsage(AttributeTargets.Method)]
public class HttpPostAttribute :Attribute
{
    public HttpPostAttribute(string EndPointName)
    {
        this.EndPointName = EndPointName;
    }
    public string EndPointName { get; private init; }
    
}