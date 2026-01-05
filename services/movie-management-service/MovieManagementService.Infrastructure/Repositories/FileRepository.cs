using Microsoft.EntityFrameworkCore;
using MovieManagementService.Application.Interfaces;
using MovieManagementService.Domain.Entities;
using MovieManagementService.Infrastructure.Data;

namespace MovieManagementService.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly MovieManagementContext _context;

    public FileRepository(MovieManagementContext context)
    {
        _context = context;
    }

    public async Task<MovieFile> CreateAsync(MovieFile file)
    {
        _context.MovieFiles.Add(file);
        await _context.SaveChangesAsync();
        return file;
    }

    public async Task<MovieFile?> GetByIdAsync(Guid id)
    {
        return await _context.MovieFiles.FindAsync(id);
    }

    public async Task<List<MovieFile>> GetByMovieIdAsync(Guid movieId)
    {
        return await _context.MovieFiles
            .Where(f => f.MovieId == movieId)
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var file = await _context.MovieFiles.FindAsync(id);
        if (file == null) return false;

        _context.MovieFiles.Remove(file);
        await _context.SaveChangesAsync();
        return true;
    }
}
