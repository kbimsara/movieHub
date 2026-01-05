using MovieManagementService.Application.DTOs;
using MovieManagementService.Application.Interfaces;
using MovieManagementService.Domain.Entities;

namespace MovieManagementService.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<MovieDto> CreateMovieAsync(CreateMovieDto dto, Guid userId)
    {
        var movie = new ManagedMovie
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Genre = dto.Genre,
            ReleaseYear = dto.ReleaseYear,
            DurationMinutes = dto.DurationMinutes,
            Rating = dto.Rating,
            PosterUrl = dto.PosterUrl,
            TrailerUrl = dto.TrailerUrl,
            Quality = dto.Quality,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            IsPublished = false,
            ViewCount = 0
        };

        var created = await _movieRepository.CreateAsync(movie);
        return MapToDto(created);
    }

    public async Task<MovieDto?> GetMovieByIdAsync(Guid id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        return movie == null ? null : MapToDto(movie);
    }

    public async Task<List<MovieDto>> GetAllMoviesAsync(bool publishedOnly = true)
    {
        var movies = await _movieRepository.GetAllAsync(publishedOnly);
        return movies.Select(MapToDto).ToList();
    }

    public async Task<List<MovieDto>> GetUserMoviesAsync(Guid userId)
    {
        var movies = await _movieRepository.GetByUserIdAsync(userId);
        return movies.Select(MapToDto).ToList();
    }

    public async Task<MovieDto> UpdateMovieAsync(Guid id, CreateMovieDto dto, Guid userId)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null || movie.CreatedBy != userId)
            throw new UnauthorizedAccessException("You can only update your own movies");

        movie.Title = dto.Title;
        movie.Description = dto.Description;
        movie.Genre = dto.Genre;
        movie.ReleaseYear = dto.ReleaseYear;
        movie.DurationMinutes = dto.DurationMinutes;
        movie.Rating = dto.Rating;
        movie.PosterUrl = dto.PosterUrl;
        movie.TrailerUrl = dto.TrailerUrl;
        movie.Quality = dto.Quality;
        movie.UpdatedAt = DateTime.UtcNow;

        var updated = await _movieRepository.UpdateAsync(movie);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteMovieAsync(Guid id, Guid userId)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null || movie.CreatedBy != userId)
            return false;

        return await _movieRepository.DeleteAsync(id);
    }

    public async Task<bool> PublishMovieAsync(Guid id, Guid userId)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null || movie.CreatedBy != userId)
            return false;

        movie.IsPublished = true;
        movie.UpdatedAt = DateTime.UtcNow;
        await _movieRepository.UpdateAsync(movie);
        return true;
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie != null)
        {
            movie.ViewCount++;
            await _movieRepository.UpdateAsync(movie);
        }
    }

    private static MovieDto MapToDto(ManagedMovie movie)
    {
        return new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            Genre = movie.Genre,
            ReleaseYear = movie.ReleaseYear,
            DurationMinutes = movie.DurationMinutes,
            Rating = movie.Rating,
            PosterUrl = movie.PosterUrl,
            TrailerUrl = movie.TrailerUrl,
            Quality = movie.Quality,
            CreatedAt = movie.CreatedAt,
            UpdatedAt = movie.UpdatedAt,
            CreatedBy = movie.CreatedBy,
            IsPublished = movie.IsPublished,
            ViewCount = movie.ViewCount,
            Files = movie.Files.Select(f => new MovieFileDto
            {
                Id = f.Id,
                MovieId = f.MovieId,
                FileName = f.FileName,
                FilePath = f.FilePath,
                FileType = f.FileType,
                FileSize = f.FileSize,
                Quality = f.Quality,
                MimeType = f.MimeType,
                UploadedAt = f.UploadedAt,
                UploadedBy = f.UploadedBy,
                IsProcessed = f.IsProcessed
            }).ToList()
        };
    }
}
