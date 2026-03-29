using MyServer.Abstraction;
using MyServer.Attributes;
using MyServer.Attributes.Methods;
using MyServer.Attributes.Parameters;
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
    
    [HttpPost("teste")]
    public ActionResult Teste([FromBody]productDto product)
    {
        return Ok($"Product {product.Name}  price {product.Price}");
    }
}

public record productDto(string Name,decimal Price);