using System;

namespace MyServer.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute;