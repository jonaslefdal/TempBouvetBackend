using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new[] { "Verdi 0", "Verdi 1", "Verdi 2" });
    }
}