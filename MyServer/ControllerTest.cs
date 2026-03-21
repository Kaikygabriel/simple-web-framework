using MyServer.Abstraction;
using MyServer.Attributes;
using MyServer.Model;
using MyServer.Model.Abstraction;

namespace MyServer;

public class ControllerTest : Controller
{
    [HttpGet("hello")]
    public ActionResult<string> Teste()
    {
        return "Hello World";
    }
    
    [HttpGet("Product")]
    public ActionResult<object> produtos()
    {
        return Ok(new
        {
            Name = "Mouse",
            Price = 12
        });
    }
    
    [HttpGet("teste")]
    public ActionResult teste()
    {
        return NotFound("Product is invalid");
    }
}