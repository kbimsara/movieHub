using Microsoft.AspNetCore.Mvc;

namespace MovieHub.Services.Movie.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MoviesController : ControllerBase
{
    private static readonly List<object> MockMovies = new()
    {
        new
        {
            id = "1",
            title = "The Shawshank Redemption",
            description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
            poster = "https://image.tmdb.org/t/p/w500/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg",
            backdrop = "https://image.tmdb.org/t/p/original/kXfqcdQKsToO0OUXHcrrNCHDBzO.jpg",
            releaseYear = 1994,
            duration = 142,
            genre = "Drama",
            rating = 9.3
        },
        new
        {
            id = "2",
            title = "The Godfather",
            description = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.",
            poster = "https://image.tmdb.org/t/p/w500/3bhkrj58Vtu7enYsRolD1fZdja1.jpg",
            backdrop = "https://image.tmdb.org/t/p/original/tmU7GeKVybMWFButWEGl2M4GeiP.jpg",
            releaseYear = 1972,
            duration = 175,
            genre = "Crime",
            rating = 9.2
        },
        new
        {
            id = "3",
            title = "The Dark Knight",
            description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests.",
            poster = "https://image.tmdb.org/t/p/w500/qJ2tW6WMUDux911r6m7haRef0WH.jpg",
            backdrop = "https://image.tmdb.org/t/p/original/hkBaDkMWbLaf8B1lsWsKX7Ew3Xq.jpg",
            releaseYear = 2008,
            duration = 152,
            genre = "Action",
            rating = 9.0
        },
        new
        {
            id = "4",
            title = "Pulp Fiction",
            description = "The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.",
            poster = "https://image.tmdb.org/t/p/w500/d5iIlFn5s0ImszYzBPb8JPIfbXD.jpg",
            backdrop = "https://image.tmdb.org/t/p/original/suaEOtk1N1sgg2MTM7oZd2cfVp3.jpg",
            releaseYear = 1994,
            duration = 154,
            genre = "Crime",
            rating = 8.9
        },
        new
        {
            id = "5",
            title = "Forrest Gump",
            description = "The presidencies of Kennedy and Johnson, the Vietnam War, and other historical events unfold from the perspective of an Alabama man.",
            poster = "https://image.tmdb.org/t/p/w500/arw2vcBveWOVZr6pxd9XTd1TdQa.jpg",
            backdrop = "https://image.tmdb.org/t/p/original/7c9UHXRJHKVi8xYf7s9zLzXnGMB.jpg",
            releaseYear = 1994,
            duration = 142,
            genre = "Drama",
            rating = 8.8
        }
    };

    [HttpGet("trending")]
    public IActionResult GetTrending()
    {
        return Ok(new
        {
            success = true,
            data = MockMovies.Take(5),
            message = "Trending movies retrieved successfully"
        });
    }

    [HttpGet("popular")]
    public IActionResult GetPopular()
    {
        return Ok(new
        {
            success = true,
            data = MockMovies.Skip(0).Take(5),
            message = "Popular movies retrieved successfully"
        });
    }

    [HttpGet("top-rated")]
    public IActionResult GetTopRated()
    {
        return Ok(new
        {
            success = true,
            data = MockMovies.Skip(0).Take(5),
            message = "Top rated movies retrieved successfully"
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetMovie(string id)
    {
        var movie = MockMovies.FirstOrDefault(m => (string)((dynamic)m).id == id);
        if (movie == null)
        {
            return NotFound(new { success = false, message = "Movie not found" });
        }

        return Ok(new
        {
            success = true,
            data = movie,
            message = "Movie retrieved successfully"
        });
    }
}
