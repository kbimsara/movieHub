using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieService.Models.Entities;

namespace MovieService.Data;

public static class SeedData
{
    public static async Task EnsureSeedDataAsync(MovieDbContext dbContext, ILogger logger, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Movies.AnyAsync(cancellationToken))
        {
            return;
        }

        var movies = new List<Movie>
        {
            new()
            {
                Title = "Neon Horizon",
                Description = "A retired smuggler is pulled back into the neon underworld to escort a scientist across a flooded megacity.",
                Poster = "https://placehold.co/600x900?text=Neon+Horizon",
                Backdrop = "https://placehold.co/1200x675?text=Neon+Horizon",
                Trailer = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                Year = DateTime.UtcNow.Year - 1,
                Duration = 124,
                Rating = 8.4,
                Quality = "4K",
                Genres = new List<string> { "Sci-Fi", "Thriller" },
                Tags = new List<string> { "cyberpunk", "heist", "ai" },
                Director = "Ava Sato",
                Views = 128_000,
                Cast = new List<CastMember>
                {
                    new() { Name = "Lena Ortiz", Character = "Vera Holt" },
                    new() { Name = "Matteo Cruz", Character = "Jax Kade" }
                }
            },
            new()
            {
                Title = "Wild Aster",
                Description = "An elite botanist leads a rescue across a terraformed moon after the colony loses contact with Earth.",
                Poster = "https://placehold.co/600x900?text=Wild+Aster",
                Backdrop = "https://placehold.co/1200x675?text=Wild+Aster",
                Year = DateTime.UtcNow.Year - 2,
                Duration = 102,
                Rating = 7.9,
                Quality = "1080p",
                Genres = new List<string> { "Adventure", "Drama" },
                Tags = new List<string> { "space", "survival" },
                Director = "Leo Zhang",
                Views = 64_500,
                Cast = new List<CastMember>
                {
                    new() { Name = "Priya Raman", Character = "Dr. Elyse Park" },
                    new() { Name = "Jonas Wolfe", Character = "Commander Huxley" }
                }
            }
        };

        dbContext.Movies.AddRange(movies);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded movie catalog with {Count} records", movies.Count);
    }
}
