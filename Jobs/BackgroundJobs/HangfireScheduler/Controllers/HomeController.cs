using Microsoft.AspNetCore.Mvc;

namespace Jobs.BackgroundJobs.HangfireScheduler.Jobs.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hangfire is working...");
    }
}