using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gateway health check endpoint
    /// </summary>
    /// <returns>Health status of the API Gateway</returns>
    [HttpGet]
    public IActionResult Get()
    {
        var response = new
        {
            Status = "Healthy",
            Service = "API Gateway",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        };

        _logger.LogInformation("Health check requested at {Timestamp}", response.Timestamp);

        return Ok(response);
    }
}
