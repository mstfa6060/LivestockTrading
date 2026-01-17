using Microsoft.AspNetCore.Mvc;

namespace BaseModules.FileProvider.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    public HomeController()
    { }

    [HttpGet]
    public string Get()
    {
        return "FileProvider Queue Worker works...";
    }
}