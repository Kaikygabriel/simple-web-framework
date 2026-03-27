namespace MyServer.Attributes.Parameters;

[AttributeUsage(AttributeTargets.Parameter)]
public class FromRouteAttribute  : Attribute
{
    public string? Value { get; }

    public FromRouteAttribute(string? value = null)
    {
        Value = value;
    }

}