using Microsoft.AspNetCore.Mvc;
using MovieHub.Shared.Kernel.Application;

namespace MovieHub.Shared.Kernel.API;

/// <summary>
/// Base API controller with common functionality
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(new ApiResponse<T>
            {
                Success = true,
                Data = result.Value
            });
        }

        return BadRequest(new ApiResponse<T>
        {
            Success = false,
            Error = result.Error
        });
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok(new ApiResponse
            {
                Success = true
            });
        }

        return BadRequest(new ApiResponse
        {
            Success = false,
            Error = result.Error
        });
    }
}

/// <summary>
/// Generic API response wrapper
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// Generic API response wrapper with data
/// </summary>
public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}
