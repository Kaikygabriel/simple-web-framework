using MyServer.Abstraction;
using MyServer.Attributes;
using MyServer.Model;

namespace MyServer;

public class ControllerTest : Controller
{
    
    [HttpGet("hello")]
    public ActionResult Teste()
    {
        return Ok("Hello World");
    }
    
    [HttpGet("Product")]
    public ActionResult produtos()
    {
        return Ok(new
        {
            Name = "Mouse",
            Price = 12
        });
    }
}