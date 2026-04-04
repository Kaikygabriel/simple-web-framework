using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MyServer.Jwt;

public class JwtHandler
{
    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    public static string GerarToken()
    {
        var secret = "minha-chave-super-secreta";

        // HEADER
        var header = new
        {
            alg = "HS256",
            typ = "JWT"
        };

        // PAYLOAD
        var payload = new
        {
            sub = "123",
            email = "teste@email.com",
            exp = DateTimeOffset.UtcNow.AddHours(2).ToUnixTimeSeconds()
        };

        var headerJson = JsonSerializer.Serialize(header);
        var payloadJson = JsonSerializer.Serialize(payload);

        var headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        var payloadBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

        var data = $"{headerBase64}.{payloadBase64}";

        // SIGNATURE
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var signature = Base64UrlEncode(signatureBytes);

        return $"{data}.{signature}";
    }
}