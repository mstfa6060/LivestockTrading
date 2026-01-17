using Microsoft.AspNetCore.Mvc;

namespace Gateways.ApiGateway.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("API Gateway works !");
    }
}