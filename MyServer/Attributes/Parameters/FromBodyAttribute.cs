using System;

namespace MyServer.Attributes.Parameters;

[AttributeUsage(AttributeTargets.Parameter)]
public class FromBodyAttribute : Attribute
{
    public string? Value { get; }

    public FromBodyAttribute(string? value = null)
    {
        Value = value;
    }

}