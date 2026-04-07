using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MyServer.Jwt.Models;

namespace MyServer.Jwt.Services;

public class JwtHandler
{

    private static string key = "akfdljfld@23412KK";
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
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var signature = Base64UrlEncode(signatureBytes);

        return $"{data}.{signature}";
    }

    public static bool Verifytoken(string token)
    {
        var parts = token.Split('.');

        if (parts.Length != 3)
            return false;
        
        var header = parts[0];
        var payload = parts[1];
        var signature = parts[2];
        var data = $"{header}.{payload}";
        
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var computedSignature = Base64UrlEncode(computedHash);

        if (!signature.Equals(computedSignature))
            return false;

        var payloadString = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        foreach (var a in payloadString.Replace("{","").Replace("}","").Split(','))
        {
            var partsPayload = a.Split(':');
            if (partsPayload[0] == "exp")
            {
                var date = Convert.ToDateTime(partsPayload[1]);
                if (date < DateTime.UtcNow)
                    return false;
            }
        }
        
        return signature.Equals(computedSignature);
    }
}