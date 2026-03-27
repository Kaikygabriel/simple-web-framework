using MyServer.Abstraction;
using MyServer.Attributes;
using MyServer.Attributes.Methods;
using MyServer.Attributes.Parameters;
using MyServer.Model.Abstraction;

namespace MyServer;

public class ControllerTest : Controller
{
    [HttpGet("hello/{teste}/")]
    public ActionResult Teste([FromRoute]string teste)
    {
        return Ok("Hello World " +teste);
    }
    
    [HttpGet("Product")] 
    public ActionResult produtos([FromQuery]int price,[FromQuery]string name)
    {
        return Ok(new
        {
            Name = name,
            Price = price
        });
    }
    
    [HttpGet("teste")]
    public ActionResult teste()
    {
        return NotFound("Product is invalid");
    }
}