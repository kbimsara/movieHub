using Microsoft.EntityFrameworkCore;
using MovieService.Data;
using MovieService.Models.DTOs;
using MovieService.Models.Entities;
using MovieService.Models.Filters;
using MovieService.Models.Requests;

namespace MovieService.Services;

public class MovieCatalogService : IMovieCatalogService
{
    private readonly MovieDbContext _dbContext;

    public MovieCatalogService(MovieDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResponse<MovieDto>> GetMoviesAsync(MovieFilter filter, CancellationToken cancellationToken)
    {
        filter.Normalize();

        var query = _dbContext.Movies
            .AsNoTracking()
            .Include(m => m.Cast.OrderBy(c => c.Name))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            var term = $"%{filter.Query.Trim()}%";
            query = query.Where(m => EF.Functions.ILike(m.Title, term) || EF.Functions.ILike(m.Description, term));
        }

        if (filter.Genres is { Count: > 0 })
        {
            query = query.Where(m => m.Genres.Any(g => filter.Genres!.Contains(g)));
        }

        if (filter.Year.HasValue)
        {
            query = query.Where(m => m.Year == filter.Year);
        }

        if (filter.Quality is { Count: > 0 })
        {
            query = query.Where(m => filter.Quality!.Contains(m.Quality));
        }

        if (filter.MinimumRating.HasValue)
        {
            query = query.Where(m => m.Rating >= filter.MinimumRating);
        }

        query = ApplySort(query, filter);

        var total = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)filter.PageSize);

        var results = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<MovieDto>
        {
            Data = results.Select(MapToDto).ToList(),
            Total = total,
            Page = filter.Page,
            Limit = filter.PageSize,
            TotalPages = totalPages
        };
    }

    public async Task<MovieDto?> GetMovieByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var movie = await _dbContext.Movies
            .AsNoTracking()
            .Include(m => m.Cast.OrderBy(c => c.Name))
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        return movie is null ? null : MapToDto(movie);
    }

    public async Task<MovieDto> CreateMovieAsync(CreateMovieRequest request, CancellationToken cancellationToken)
    {
        var movie = new Movie
        {
            Title = request.Title?.Trim() ?? string.Empty,
            Description = request.Description?.Trim() ?? string.Empty,
            Year = request.Year,
            Duration = request.Duration,
            Quality = request.Quality,
            Rating = request.Rating,
            Genres = NormalizeList(request.Genres),
            Tags = NormalizeList(request.Tags),
            Director = request.Director,
            Poster = request.Poster,
            Backdrop = request.Backdrop,
            Trailer = request.Trailer,
            StreamUrl = request.StreamUrl,
            DownloadUrl = request.DownloadUrl,
            TorrentMagnet = request.TorrentMagnet,
            Cast = (request.Cast ?? new List<CastMemberRequest>()).Select(ToEntity).ToList(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Movies.Add(movie);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(movie);
    }

    public async Task<MovieDto?> UpdateMovieAsync(Guid id, UpdateMovieRequest request, CancellationToken cancellationToken)
    {
        var movie = await _dbContext.Movies
            .Include(m => m.Cast)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (movie is null)
        {
            return null;
        }

        movie.Title = request.Title?.Trim() ?? movie.Title;
        movie.Description = request.Description?.Trim() ?? movie.Description;
        movie.Year = request.Year;
        movie.Duration = request.Duration;
        movie.Quality = request.Quality;
        movie.Rating = request.Rating;
        movie.Genres = NormalizeList(request.Genres);
        movie.Tags = NormalizeList(request.Tags);
        movie.Director = request.Director;
        movie.Poster = request.Poster;
        movie.Backdrop = request.Backdrop;
        movie.Trailer = request.Trailer;
        movie.StreamUrl = request.StreamUrl;
        movie.DownloadUrl = request.DownloadUrl;
        movie.TorrentMagnet = request.TorrentMagnet;
        if (request.Views.HasValue)
        {
            movie.Views = request.Views.Value;
        }
        movie.UpdatedAt = DateTime.UtcNow;

        _dbContext.CastMembers.RemoveRange(movie.Cast);
        movie.Cast = (request.Cast ?? new List<CastMemberRequest>()).Select(ToEntity).ToList();

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Entry(movie).Collection(m => m.Cast).LoadAsync(cancellationToken);

        return MapToDto(movie);
    }

    public async Task<bool> DeleteMovieAsync(Guid id, CancellationToken cancellationToken)
    {
        var movie = await _dbContext.Movies.FindAsync(new object?[] { id }, cancellationToken);
        if (movie is null)
        {
            return false;
        }

        _dbContext.Movies.Remove(movie);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<MovieDto>> GetRelatedMoviesAsync(Guid id, CancellationToken cancellationToken)
    {
        var movie = await _dbContext.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (movie is null || movie.Genres.Count == 0)
        {
            return Array.Empty<MovieDto>();
        }

        var related = await _dbContext.Movies
            .AsNoTracking()
            .Include(m => m.Cast.OrderBy(c => c.Name))
            .Where(m => m.Id != id && m.Genres.Any(g => movie.Genres.Contains(g)))
            .OrderByDescending(m => m.Rating)
            .ThenByDescending(m => m.Views)
            .Take(10)
            .ToListAsync(cancellationToken);

        return related.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<string>> GetGenresAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Movies
            .AsNoTracking()
            .SelectMany(m => m.Genres)
            .Distinct()
            .OrderBy(g => g)
            .ToListAsync(cancellationToken);
    }

    private static List<string> NormalizeList(IEnumerable<string>? items)
        => items?
               .Select(i => i.Trim())
               .Where(i => !string.IsNullOrWhiteSpace(i))
               .Distinct(StringComparer.OrdinalIgnoreCase)
               .ToList()
           ?? new List<string>();

    private static CastMember ToEntity(CastMemberRequest request)
        => new()
        {
            Name = request.Name.Trim(),
            Character = request.Character?.Trim(),
            Photo = request.Photo
        };

    private static IQueryable<Movie> ApplySort(IQueryable<Movie> query, MovieFilter filter)
    {
        var sortDescending = !string.Equals(filter.SortOrder, "asc", StringComparison.OrdinalIgnoreCase);

        return filter.SortBy?.ToLowerInvariant() switch
        {
            "title" => sortDescending ? query.OrderByDescending(m => m.Title) : query.OrderBy(m => m.Title),
            "year" => sortDescending ? query.OrderByDescending(m => m.Year) : query.OrderBy(m => m.Year),
            "rating" => sortDescending ? query.OrderByDescending(m => m.Rating) : query.OrderBy(m => m.Rating),
            "views" => sortDescending ? query.OrderByDescending(m => m.Views) : query.OrderBy(m => m.Views),
            "createdat" => sortDescending ? query.OrderByDescending(m => m.CreatedAt) : query.OrderBy(m => m.CreatedAt),
            _ => sortDescending ? query.OrderByDescending(m => m.CreatedAt) : query.OrderBy(m => m.CreatedAt)
        };
    }

    private static MovieDto MapToDto(Movie movie)
        => new(
            movie.Id,
            movie.Title,
            movie.Description,
            movie.Poster,
            movie.Backdrop,
            movie.Trailer,
            movie.StreamUrl,
            movie.DownloadUrl,
            movie.TorrentMagnet,
            movie.Year,
            movie.Duration,
            movie.Rating,
            movie.Quality,
            movie.Genres,
            movie.Tags,
            movie.Director,
            movie.Views,
            movie.CreatedAt,
            movie.UpdatedAt,
            movie.Cast
                .OrderBy(c => c.Name)
                .Select(c => new CastMemberDto(c.Id, c.Name, c.Character, c.Photo))
                .ToList()
        );
}
