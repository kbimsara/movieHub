using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieHub.AuthService.Dtos;
using MovieHub.AuthService.Extensions;
using MovieHub.AuthService.Models;
using MovieHub.AuthService.Options;
using MovieHub.AuthService.Services;
using MovieHub.AuthService.Stores;

namespace MovieHub.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserStore _userStore;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserStore userStore,
        IRefreshTokenStore refreshTokenStore,
        ITokenService tokenService,
        IPasswordHasher<User> passwordHasher,
        IOptions<JwtOptions> jwtOptions,
        ILogger<AuthController> logger)
    {
        _userStore = userStore;
        _refreshTokenStore = refreshTokenStore;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Invalid request payload", BuildModelStateErrorMessage()));
        }

        var email = request.Email.Trim().ToLowerInvariant();
        if (await _userStore.EmailExistsAsync(email, HttpContext.RequestAborted))
        {
            return Conflict(ApiResponse<object>.Fail("Email already registered"));
        }

        var user = new User
        {
            Email = email,
            Username = ResolveUsername(request),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Role = "user",
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userStore.AddAsync(user, HttpContext.RequestAborted);
        var (accessToken, refreshToken) = await IssueTokensAsync(user, HttpContext.RequestAborted);

        return Ok(new
        {
            token = accessToken,
            refreshToken = refreshToken.Token,
            expiresAt = CalculateAccessExpiry()
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Invalid request payload", BuildModelStateErrorMessage()));
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userStore.GetByEmailAsync(email, HttpContext.RequestAborted);
        if (user is null)
        {
            return Unauthorized(ApiResponse<object>.Fail("Invalid email or password"));
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return Unauthorized(ApiResponse<object>.Fail("Invalid email or password"));
        }

        var (accessToken, refreshToken) = await IssueTokensAsync(user, HttpContext.RequestAborted);
        return Ok(new
        {
            token = accessToken,
            refreshToken = refreshToken.Token,
            expiresAt = CalculateAccessExpiry()
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest? request)
    {
        var cancellationToken = HttpContext.RequestAborted;
        if (!string.IsNullOrWhiteSpace(request?.RefreshToken))
        {
            await _refreshTokenStore.InvalidateAsync(request.RefreshToken, cancellationToken);
        }
        else
        {
            var userId = User.GetUserId();
            if (userId.HasValue)
            {
                await _refreshTokenStore.InvalidateAllForUserAsync(userId.Value, cancellationToken);
            }
        }

        var payload = new { message = "Logged out" };
        return Ok(ApiResponse<object>.Ok(payload, "Logout successful"));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(ApiResponse<UserResponse>.Fail("Invalid token"));
        }

        var user = await _userStore.GetByIdAsync(userId.Value, HttpContext.RequestAborted);
        if (user is null)
        {
            return Unauthorized(ApiResponse<UserResponse>.Fail("User not found"));
        }

        return Ok(ApiResponse<UserResponse>.Ok(MapUser(user)));
    }

    [Authorize]
    [HttpGet("validate")]
    public IActionResult Validate()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(ApiResponse<ValidateTokenResponse>.Fail("Invalid token"));
        }

        var response = new ValidateTokenResponse
        {
            Valid = true,
            UserId = userId.Value.ToString(),
            Email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(JwtRegisteredClaimNames.Email) ?? string.Empty,
            Username = User.FindFirstValue(JwtRegisteredClaimNames.UniqueName) ?? User.Identity?.Name ?? string.Empty
        };

        return Ok(ApiResponse<ValidateTokenResponse>.Ok(response));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<RefreshTokenResponse>.Fail("Invalid request payload", BuildModelStateErrorMessage()));
        }

        var storedToken = await _refreshTokenStore.GetAsync(request.RefreshToken, HttpContext.RequestAborted);
        if (storedToken is null || !storedToken.IsActive)
        {
            return Unauthorized(ApiResponse<RefreshTokenResponse>.Fail("Invalid or expired refresh token"));
        }

        var user = await _userStore.GetByIdAsync(storedToken.UserId, HttpContext.RequestAborted);
        if (user is null)
        {
            return Unauthorized(ApiResponse<RefreshTokenResponse>.Fail("User not found"));
        }

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        string? rotatedRefreshToken = null;

        if (storedToken.ExpiresAt <= DateTime.UtcNow.AddDays(1))
        {
            storedToken.Revoke();
            var replacement = _tokenService.GenerateRefreshToken(user.Id);
            await _refreshTokenStore.StoreAsync(replacement, HttpContext.RequestAborted);
            rotatedRefreshToken = replacement.Token;
        }

        var data = new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = rotatedRefreshToken
        };

        return Ok(ApiResponse<RefreshTokenResponse>.Ok(data));
    }

    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Invalid request payload", BuildModelStateErrorMessage()));
        }

        _logger.LogInformation("Password reset requested for {Email}", request.Email.Trim().ToLowerInvariant());
        var data = new { message = "If the account exists, a reset email has been sent." };
        return Ok(ApiResponse<object>.Ok(data));
    }

    [HttpPost("reset-password")]
    public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Invalid request payload", BuildModelStateErrorMessage()));
        }

        var data = new { message = "Password reset successfully." };
        return Ok(ApiResponse<object>.Ok(data));
    }

    private async Task<(string accessToken, RefreshToken refreshToken)> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);
        await _refreshTokenStore.StoreAsync(refreshToken, cancellationToken);
        return (accessToken, refreshToken);
    }

    private static string ResolveUsername(RegisterRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            return request.Username.Trim().ToLowerInvariant();
        }

        var slugFromEmail = request.Email.Split('@')[0];
        var fallback = $"{request.FirstName}.{request.LastName}".Replace(" ", string.Empty);
        return (string.IsNullOrWhiteSpace(slugFromEmail) ? fallback : slugFromEmail).ToLowerInvariant();
    }

    private string CalculateAccessExpiry()
    {
        return DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenLifetimeMinutes).ToString("O");
    }

    private static UserResponse MapUser(User user)
    {
        return new UserResponse
        {
            Id = user.Id.ToString(),
            Email = user.Email,
            Username = user.Username,
            Avatar = user.AvatarUrl,
            Role = user.Role,
            CreatedAt = user.CreatedAt.ToString("O")
        };
    }

    private string BuildModelStateErrorMessage()
    {
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        return string.Join("; ", errors);
    }
}
