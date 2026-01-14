using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.API.Data;
using Library.API.DTOs;
using LibraryModel = Library.API.Models.UserLibrary;

namespace Library.API.Controllers;

[ApiController]
[Route("api/libraries")]
[Authorize]
public class LibrariesController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public LibrariesController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserLibraries()
    {
        // In a real implementation, you'd get the user ID from the JWT token
        // For now, we'll return all libraries (this should be filtered by user)
        var libraries = await _context.UserLibraries
            .Include(l => l.Items)
            .ToListAsync();

        var dtos = libraries.Select(l => new UserLibraryDto
        {
            Id = l.Id,
            UserId = l.UserId,
            Name = l.Name,
            Description = l.Description,
            IsDefault = l.IsDefault,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt,
            ItemCount = l.Items.Count
        });

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLibrary(Guid id)
    {
        var library = await _context.UserLibraries
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (library == null) return NotFound();

        var dto = new UserLibraryDto
        {
            Id = library.Id,
            UserId = library.UserId,
            Name = library.Name,
            Description = library.Description,
            IsDefault = library.IsDefault,
            CreatedAt = library.CreatedAt,
            UpdatedAt = library.UpdatedAt,
            ItemCount = library.Items.Count
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLibrary([FromBody] CreateUserLibraryDto dto)
    {
        // In a real implementation, get user ID from JWT token
        var userId = Guid.NewGuid(); // Placeholder

        var library = new LibraryModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
            IsDefault = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.UserLibraries.Add(library);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLibrary), new { id = library.Id }, library);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLibrary(Guid id, [FromBody] CreateUserLibraryDto dto)
    {
        var library = await _context.UserLibraries.FindAsync(id);
        if (library == null) return NotFound();

        library.Name = dto.Name;
        library.Description = dto.Description;
        library.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLibrary(Guid id)
    {
        var library = await _context.UserLibraries.FindAsync(id);
        if (library == null) return NotFound();

        _context.UserLibraries.Remove(library);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}