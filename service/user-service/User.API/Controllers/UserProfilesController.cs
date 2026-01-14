using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.API.Data;
using User.API.DTOs;
using UserProfileModel = User.API.Models.UserProfile;

namespace User.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserProfilesController : ControllerBase
{
    private readonly UserDbContext _context;

    public UserProfilesController(UserDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserProfiles()
    {
        var userProfiles = await _context.UserProfiles.ToListAsync();
        var dtos = userProfiles.Select(u => new UserProfileDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Bio = u.Bio,
            AvatarUrl = u.AvatarUrl,
            DateOfBirth = u.DateOfBirth,
            Country = u.Country,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        });
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserProfile(Guid id)
    {
        var userProfile = await _context.UserProfiles.FindAsync(id);
        if (userProfile == null) return NotFound();
        var dto = new UserProfileDto
        {
            Id = userProfile.Id,
            Username = userProfile.Username,
            Email = userProfile.Email,
            FirstName = userProfile.FirstName,
            LastName = userProfile.LastName,
            Bio = userProfile.Bio,
            AvatarUrl = userProfile.AvatarUrl,
            DateOfBirth = userProfile.DateOfBirth,
            Country = userProfile.Country,
            CreatedAt = userProfile.CreatedAt,
            UpdatedAt = userProfile.UpdatedAt
        };
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserProfile([FromBody] CreateUserProfileDto dto)
    {
        // Check if username or email already exists
        var existingUser = await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.Username == dto.Username || u.Email == dto.Email);
        if (existingUser != null)
        {
            return Conflict("Username or email already exists");
        }

        var userProfile = new UserProfileModel
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Bio = dto.Bio,
            AvatarUrl = dto.AvatarUrl,
            DateOfBirth = dto.DateOfBirth,
            Country = dto.Country,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.UserProfiles.Add(userProfile);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserProfile), new { id = userProfile.Id }, userProfile);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserProfile(Guid id, [FromBody] UpdateUserProfileDto dto)
    {
        var userProfile = await _context.UserProfiles.FindAsync(id);
        if (userProfile == null) return NotFound();

        if (!string.IsNullOrEmpty(dto.FirstName)) userProfile.FirstName = dto.FirstName;
        if (!string.IsNullOrEmpty(dto.LastName)) userProfile.LastName = dto.LastName;
        if (dto.Bio != null) userProfile.Bio = dto.Bio;
        if (dto.AvatarUrl != null) userProfile.AvatarUrl = dto.AvatarUrl;
        if (dto.DateOfBirth.HasValue) userProfile.DateOfBirth = dto.DateOfBirth.Value;
        if (dto.Country != null) userProfile.Country = dto.Country;

        userProfile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserProfile(Guid id)
    {
        var userProfile = await _context.UserProfiles.FindAsync(id);
        if (userProfile == null) return NotFound();
        _context.UserProfiles.Remove(userProfile);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}