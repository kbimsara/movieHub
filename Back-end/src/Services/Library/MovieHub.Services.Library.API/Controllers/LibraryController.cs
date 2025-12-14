using Microsoft.AspNetCore.Mvc;

namespace MovieHub.Services.Library.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LibraryController : ControllerBase
{
    private static readonly List<object> MockContinueWatching = new()
    {
        new
        {
            id = "1",
            movieId = "1",
            userId = "user1",
            progress = 45,
            lastWatched = DateTime.UtcNow.AddHours(-2),
            movie = new
            {
                id = "1",
                title = "The Shawshank Redemption",
                poster = "https://image.tmdb.org/t/p/w500/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg",
                duration = 142
            }
        },
        new
        {
            id = "2",
            movieId = "3",
            userId = "user1",
            progress = 67,
            lastWatched = DateTime.UtcNow.AddDays(-1),
            movie = new
            {
                id = "3",
                title = "The Dark Knight",
                poster = "https://image.tmdb.org/t/p/w500/qJ2tW6WMUDux911r6m7haRef0WH.jpg",
                duration = 152
            }
        }
    };

    [HttpGet("continue-watching")]
    public IActionResult GetContinueWatching()
    {
        return Ok(new
        {
            success = true,
            data = MockContinueWatching,
            message = "Continue watching retrieved successfully"
        });
    }

    [HttpGet("favorites")]
    public IActionResult GetFavorites()
    {
        return Ok(new
        {
            success = true,
            data = new List<object>(),
            message = "Favorites retrieved successfully"
        });
    }

    [HttpGet]
    public IActionResult GetLibrary()
    {
        return Ok(new
        {
            success = true,
            data = new List<object>(),
            message = "Library retrieved successfully"
        });
    }

    [HttpPost]
    public IActionResult AddToLibrary([FromBody] dynamic request)
    {
        return Ok(new
        {
            success = true,
            data = new { id = Guid.NewGuid().ToString(), movieId = request.movieId },
            message = "Added to library successfully"
        });
    }

    [HttpPut("{movieId}/progress")]
    public IActionResult UpdateProgress(string movieId, [FromBody] dynamic request)
    {
        return Ok(new
        {
            success = true,
            data = new { movieId, progress = request.progress },
            message = "Progress updated successfully"
        });
    }
}
