using Microsoft.AspNetCore.Mvc;
using MovieSearchService.Application.Interfaces;

namespace MovieSearchService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ISearchRepository _searchRepository;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        ISearchRepository searchRepository,
        ILogger<HealthController> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> GetHealth()
    {
        try
        {
            var isHealthy = await _searchRepository.PingAsync();

            if (isHealthy)
            {
                return Ok(new
                {
                    status = "healthy",
                    service = "Movie Search Service",
                    elasticsearch = "connected"
                });
            }

            return StatusCode(503, new
            {
                status = "unhealthy",
                service = "Movie Search Service",
                elasticsearch = "disconnected"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new
            {
                status = "unhealthy",
                service = "Movie Search Service",
                error = ex.Message
            });
        }
    }
}
