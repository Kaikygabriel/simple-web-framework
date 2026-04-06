using System;
using System.Collections.Generic;
using System.Text;
using MyServer.Abstraction;
using MyServer.Attributes;
using MyServer.Attributes.Methods;
using MyServer.Attributes.Parameters;
using MyServer.Jwt.Models;
using MyServer.Jwt.Services;
using MyServer.Model.Abstraction;

namespace MyServer;

public class ControllerTest : Controller
{
    [HttpGet("hello/{nome}/{sobreNome}")]
    public ActionResult Teste([FromRoute]string nome,[FromRoute]string sobreNome)
    {
        return Ok("Seu nome é  " +nome +" e sobrenome é " + sobreNome);
    }
        
    [HttpGet("Product/{name}")] 
    public ActionResult produtos([FromQuery]int price,[FromRoute]string name)
    {
        return Ok(new
        {
            Name = name, 
            Price = price
        });
    }
    
    [HttpPost("Register")]
    public ActionResult Register([FromBody]string name)
    {
        var claims = new List<ClaimsType>()
        {
            new("exp", DateTime.UtcNow.AddHours(1) ),
            new("name", name),
            new("Idade",18),
        };
        var tokenDescriptor = new TokenDescriptor(claims,Encoding.UTF8.GetBytes("teste1234@teste@teste"));
        var token = JwtHandler.CreateToken(tokenDescriptor);
        return Ok(token);
    }
}

public record productDto(string Name,decimal Price);