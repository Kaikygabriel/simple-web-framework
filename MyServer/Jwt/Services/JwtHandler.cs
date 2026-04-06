using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MyServer.Jwt.Models;

namespace MyServer.Jwt.Services;

public class JwtHandler
{
    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    public static string CreateToken(TokenDescriptor tokenDescriptor)
    {
        var header = new
        {
            alg = "HS256",
            typ = "JWT"
        };
        
        var headerJson = JsonSerializer.Serialize(header);
        var payloadJson = ClaimsType.ToString(tokenDescriptor.Claims);

        var headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        var payloadBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

        var data = $"{headerBase64}.{payloadBase64}";

        // SIGNATURE
        using var hmac = new HMACSHA256(tokenDescriptor.Key);
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var signature = Base64UrlEncode(signatureBytes);

        return $"{data}.{signature}";
    }
}