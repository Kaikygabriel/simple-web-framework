using System.Collections.Generic;
using System.Linq;
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
            stringBuilder.Append($""" "{claim.Key}" : {JsonSerializer.Serialize(claim.Value)} """);
            if (!claim.Equals(claims.LastOrDefault()))
                stringBuilder.AppendLine(",");
        }
        
        stringBuilder.AppendLine("}");
        
        return stringBuilder.ToString();
    }
};