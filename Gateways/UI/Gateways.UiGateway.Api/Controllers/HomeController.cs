using Microsoft.AspNetCore.Mvc;

namespace Gateways.UiGateway.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("UI Gateway works !");
    }
}