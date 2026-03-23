namespace MyServer.Attributes.Parameters;

[AttributeUsage(AttributeTargets.Parameter)]
public class FromQueryAttribute : Attribute
{
    public string? Value { get; }

    public FromQueryAttribute(string? value = null)
    {
        Value = value;
    }
    
    
}