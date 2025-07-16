using Microsoft.AspNetCore.Mvc;

namespace ParkManager_Service.Controllers;

[ApiController]
[Route("/")]
public class HelloController() : ControllerBase
{
    [HttpGet(Name = "GetHello")]
    public string Get()
    {
        return "ParkManager API!";
    }
}
