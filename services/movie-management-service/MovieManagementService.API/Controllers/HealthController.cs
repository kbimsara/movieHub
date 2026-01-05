using Microsoft.AspNetCore.Mvc;

namespace MovieManagementService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            service = "Movie Management Service",
            timestamp = DateTime.UtcNow
        });
    }
}
