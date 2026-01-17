using Microsoft.AspNetCore.Mvc;

namespace Gateways.ApiGateway.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthCheckController : ControllerBase
{
	[HttpGet]
	public IActionResult Get()
	{
		return Ok("success");
	}
}
