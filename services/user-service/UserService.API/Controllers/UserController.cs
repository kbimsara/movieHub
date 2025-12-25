using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.API.Controllers;

/// <summary>
/// User Profile Controller
/// All endpoints require JWT authentication
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize] // ALL endpoints require authentication
public class UserController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;

    public UserController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    /// <summary>
    /// Get current user's profile
    /// Reads UserId from JWT token claims
    /// </summary>
    /// <returns>User profile or 404 if not found</returns>
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponseDto>> GetMyProfile()
    {
        // Extract UserId from JWT token (sub claim)
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid token: User ID not found" });
        }

        var profile = await _userProfileService.GetUserProfileAsync(userId);
        
        if (profile == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        return Ok(profile);
    }

    /// <summary>
    /// Create a user profile
    /// Reads UserId and Email from JWT token claims
    /// </summary>
    /// <param name="request">Display name for the user</param>
    /// <returns>Created user profile</returns>
    [HttpPost]
    public async Task<ActionResult<UserProfileResponseDto>> CreateUserProfile(
        [FromBody] CreateUserProfileRequestDto request)
    {
        // Extract UserId from JWT token (sub claim)
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid token: User ID not found" });
        }

        // Extract Email from JWT token (email claim)
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized(new { message = "Invalid token: Email not found" });
        }

        try
        {
            var profile = await _userProfileService.CreateUserProfileAsync(
                userId, 
                email, 
                request.DisplayName);

            return CreatedAtAction(
                nameof(GetMyProfile), 
                new { id = profile.Id }, 
                profile);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
