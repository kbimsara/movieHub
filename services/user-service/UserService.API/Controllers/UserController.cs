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
[Route("api")]
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
        // Note: Using "sub" directly because DefaultInboundClaimTypeMap was cleared
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
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
        // Note: Using "sub" directly because DefaultInboundClaimTypeMap was cleared
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid token: User ID not found" });
        }

        // Extract Email from JWT token (email claim)
        // Note: Using "email" directly because DefaultInboundClaimTypeMap was cleared
        var email = User.FindFirst("email")?.Value ?? User.FindFirst(ClaimTypes.Email)?.Value;
        
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

    /// <summary>
    /// Update current user's profile
    /// Reads UserId from JWT token claims
    /// </summary>
    /// <param name="request">Fields to update</param>
    /// <returns>Updated user profile</returns>
    [HttpPut("me")]
    public async Task<ActionResult<UserProfileResponseDto>> UpdateMyProfile(
        [FromBody] UpdateUserProfileRequestDto request)
    {
        // Extract UserId from JWT token (sub claim)
        // Note: Using "sub" directly because DefaultInboundClaimTypeMap was cleared
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid token: User ID not found" });
        }

        var profile = await _userProfileService.UpdateUserProfileAsync(userId, request);
        
        if (profile == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        return Ok(profile);
    }
    [HttpGet("me/history")]
    public IActionResult GetHistory()
    {
        return Ok(new object[] { });
    }

    [HttpGet("me/library")]
    public IActionResult GetLibrary()
    {
        return Ok(new object[] { });
    }

    [HttpGet("me/watch-later")]
    public IActionResult GetWatchLater()
    {
        return Ok(new object[] { });
    }

    [HttpGet("me/favorites")]
    public IActionResult GetFavorites()
    {
        return Ok(new object[] { });
    }
}
