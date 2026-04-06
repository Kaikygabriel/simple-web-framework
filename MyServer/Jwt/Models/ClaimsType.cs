using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MyServer.Jwt.Models;

public record ClaimsType(string Key, dynamic Value)
{
    public static string ToString(List<ClaimsType> claims)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("{");
        foreach (var claim in claims)
        {
            stringBuilder.AppendLine($""" "{claim.Key}" : {JsonSerializer.Serialize(claim.Value)},  """);
        }
        stringBuilder.AppendLine("}");
        
        return stringBuilder.ToString();
    }
};