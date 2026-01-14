using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.API.Data;
using Library.API.DTOs;
using LibraryItemModel = Library.API.Models.LibraryItem;

namespace Library.API.Controllers;

[ApiController]
[Route("api/libraries/{libraryId}/items")]
[Authorize]
public class LibraryItemsController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public LibraryItemsController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetLibraryItems(Guid libraryId)
    {
        var library = await _context.UserLibraries.FindAsync(libraryId);
        if (library == null) return NotFound("Library not found");

        var items = await _context.LibraryItems
            .Where(i => i.LibraryId == libraryId)
            .OrderByDescending(i => i.AddedAt)
            .ToListAsync();

        var dtos = items.Select(i => new LibraryItemDto
        {
            Id = i.Id,
            LibraryId = i.LibraryId,
            MovieId = i.MovieId,
            MovieTitle = i.MovieTitle,
            MoviePosterUrl = i.MoviePosterUrl,
            MovieReleaseYear = i.MovieReleaseYear,
            AddedAt = i.AddedAt,
            Notes = i.Notes,
            Rating = i.Rating
        });

        return Ok(dtos);
    }

    [HttpGet("{itemId}")]
    public async Task<IActionResult> GetLibraryItem(Guid libraryId, Guid itemId)
    {
        var item = await _context.LibraryItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.LibraryId == libraryId);

        if (item == null) return NotFound();

        var dto = new LibraryItemDto
        {
            Id = item.Id,
            LibraryId = item.LibraryId,
            MovieId = item.MovieId,
            MovieTitle = item.MovieTitle,
            MoviePosterUrl = item.MoviePosterUrl,
            MovieReleaseYear = item.MovieReleaseYear,
            AddedAt = item.AddedAt,
            Notes = item.Notes,
            Rating = item.Rating
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> AddMovieToLibrary(Guid libraryId, [FromBody] AddMovieToLibraryDto dto)
    {
        var library = await _context.UserLibraries.FindAsync(libraryId);
        if (library == null) return NotFound("Library not found");

        // Check if movie already exists in this library
        var existingItem = await _context.LibraryItems
            .FirstOrDefaultAsync(i => i.LibraryId == libraryId && i.MovieId == dto.MovieId);

        if (existingItem != null)
        {
            return Conflict("Movie already exists in this library");
        }

        var item = new LibraryItemModel
        {
            Id = Guid.NewGuid(),
            LibraryId = libraryId,
            MovieId = dto.MovieId,
            MovieTitle = dto.MovieTitle,
            MoviePosterUrl = dto.MoviePosterUrl,
            MovieReleaseYear = dto.MovieReleaseYear,
            AddedAt = DateTime.UtcNow,
            Notes = dto.Notes,
            Rating = dto.Rating
        };

        _context.LibraryItems.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLibraryItem),
            new { libraryId = libraryId, itemId = item.Id }, item);
    }

    [HttpPut("{itemId}")]
    public async Task<IActionResult> UpdateLibraryItem(Guid libraryId, Guid itemId, [FromBody] AddMovieToLibraryDto dto)
    {
        var item = await _context.LibraryItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.LibraryId == libraryId);

        if (item == null) return NotFound();

        item.Notes = dto.Notes;
        item.Rating = dto.Rating;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{itemId}")]
    public async Task<IActionResult> RemoveFromLibrary(Guid libraryId, Guid itemId)
    {
        var item = await _context.LibraryItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.LibraryId == libraryId);

        if (item == null) return NotFound();

        _context.LibraryItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}