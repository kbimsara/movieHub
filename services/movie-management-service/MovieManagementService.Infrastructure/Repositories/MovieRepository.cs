using Microsoft.EntityFrameworkCore;
using MovieManagementService.Application.Interfaces;
using MovieManagementService.Domain.Entities;
using MovieManagementService.Infrastructure.Data;

namespace MovieManagementService.Infrastructure.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly MovieManagementContext _context;

    public MovieRepository(MovieManagementContext context)
    {
        _context = context;
    }

    public async Task<ManagedMovie> CreateAsync(ManagedMovie movie)
    {
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task<ManagedMovie?> GetByIdAsync(Guid id)
    {
        return await _context.Movies
            .Include(m => m.Files)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<ManagedMovie>> GetAllAsync(bool publishedOnly = true)
    {
        var query = _context.Movies.Include(m => m.Files).AsQueryable();
        
        if (publishedOnly)
        {
            query = query.Where(m => m.IsPublished);
        }
        
        return await query.OrderByDescending(m => m.CreatedAt).ToListAsync();
    }

    public async Task<List<ManagedMovie>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Movies
            .Include(m => m.Files)
            .Where(m => m.CreatedBy == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<ManagedMovie> UpdateAsync(ManagedMovie movie)
    {
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return false;

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return true;
    }
}
