using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Extensions;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Models.Requests;
using UserService.Services.Interfaces;

namespace UserService.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public class MeController : ControllerBase
{
    private readonly IUserProfileService _profileService;
    private readonly IUserDataService _userDataService;
    private readonly IMovieMetadataClient _movieMetadataClient;
    private readonly ILogger<MeController> _logger;

    public MeController(
        IUserProfileService profileService,
        IUserDataService userDataService,
        IMovieMetadataClient movieMetadataClient,
        ILogger<MeController> logger)
    {
        _profileService = profileService;
        _userDataService = userDataService;
        _movieMetadataClient = movieMetadataClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<UserProfileDto>.Fail("unauthorized"));
        }

        var profile = await _profileService.GetOrCreateAsync(userId.Value, User, cancellationToken);
        return Ok(ApiResponse<UserProfileDto>.Ok(MapProfile(profile)));
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateProfile(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<UserProfileDto>.Fail("unauthorized"));
        }

        var profile = await _profileService.UpdateProfileAsync(userId.Value, request, cancellationToken);
        return Ok(ApiResponse<UserProfileDto>.Ok(MapProfile(profile), "Profile updated"));
    }

    [HttpPost("avatar")]
    [RequestSizeLimit(6_000_000)]
    public async Task<ActionResult<ApiResponse<AvatarUploadDto>>> UploadAvatar([FromForm] IFormFile? avatar, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<AvatarUploadDto>.Fail("unauthorized"));
        }

        if (avatar is null || avatar.Length == 0)
        {
            return BadRequest(ApiResponse<AvatarUploadDto>.Fail("Avatar file is required"));
        }

        // TODO: integrate with dedicated storage service
        var avatarUrl = $"https://api.dicebear.com/7.x/initials/svg?seed={Guid.NewGuid()}";
        await _profileService.UpdateAvatarAsync(userId.Value, avatarUrl, cancellationToken);
        return Ok(ApiResponse<AvatarUploadDto>.Ok(new AvatarUploadDto(avatarUrl), "Avatar updated"));
    }

    [HttpGet("library")]
    public async Task<ActionResult<ApiResponse<IEnumerable<LibraryItemDto>>>> GetLibrary(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<IEnumerable<LibraryItemDto>>.Fail("unauthorized"));
        }

        var items = await _userDataService.GetLibraryAsync(userId.Value, cancellationToken);
        var movieMap = await _movieMetadataClient.GetMoviesAsync(items.Select(x => x.MovieId), cancellationToken);
        var response = items.Select(item => MapLibraryItem(item, movieMap.TryGetValue(item.MovieId, out var movie) ? movie : null));

        return Ok(ApiResponse<IEnumerable<LibraryItemDto>>.Ok(response));
    }

    [HttpPost("library")]
    public async Task<ActionResult<ApiResponse<LibraryItemDto>>> AddToLibrary(AddToLibraryRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<LibraryItemDto>.Fail("unauthorized"));
        }

        var item = await _userDataService.AddToLibraryAsync(userId.Value, request.MovieId, cancellationToken);
        var movie = await _movieMetadataClient.GetMovieAsync(item.MovieId, cancellationToken);
        return Ok(ApiResponse<LibraryItemDto>.Ok(MapLibraryItem(item, movie), "Added to library"));
    }

    [HttpDelete("library/{movieId:guid}")]
    public async Task<ActionResult<ApiResponse<string?>>> RemoveFromLibrary(Guid movieId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<string?>.Fail("unauthorized"));
        }

        var removed = await _userDataService.RemoveFromLibraryAsync(userId.Value, movieId, cancellationToken);
        if (!removed)
        {
            return NotFound(ApiResponse<string?>.Fail("Movie not found in library"));
        }

        return Ok(ApiResponse<string?>.Ok(null, "Removed from library"));
    }

    [HttpPut("library/{movieId:guid}/progress")]
    public async Task<ActionResult<ApiResponse<LibraryItemDto>>> UpdateProgress(Guid movieId, UpdateProgressRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<LibraryItemDto>.Fail("unauthorized"));
        }

        var item = await _userDataService.UpdateProgressAsync(userId.Value, movieId, request.Progress, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<LibraryItemDto>.Fail("Movie not found in library"));
        }

        var movie = await _movieMetadataClient.GetMovieAsync(movieId, cancellationToken);
        return Ok(ApiResponse<LibraryItemDto>.Ok(MapLibraryItem(item, movie), "Progress updated"));
    }

    [HttpPost("library/{movieId:guid}/favorite")]
    public async Task<ActionResult<ApiResponse<LibraryItemDto>>> ToggleFavorite(Guid movieId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<LibraryItemDto>.Fail("unauthorized"));
        }

        var item = await _userDataService.ToggleFavoriteAsync(userId.Value, movieId, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<LibraryItemDto>.Fail("Movie not found in library"));
        }

        var movie = await _movieMetadataClient.GetMovieAsync(movieId, cancellationToken);
        return Ok(ApiResponse<LibraryItemDto>.Ok(MapLibraryItem(item, movie), "Favorite state updated"));
    }

    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WatchHistoryDto>>>> GetHistory(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<IEnumerable<WatchHistoryDto>>.Fail("unauthorized"));
        }

        var history = await _userDataService.GetWatchHistoryAsync(userId.Value, cancellationToken);
        var movieMap = await _movieMetadataClient.GetMoviesAsync(history.Select(x => x.MovieId), cancellationToken);
        var response = history.Select(entry => MapHistoryEntry(entry, movieMap.TryGetValue(entry.MovieId, out var movie) ? movie : null));
        return Ok(ApiResponse<IEnumerable<WatchHistoryDto>>.Ok(response));
    }

    [HttpPost("history")]
    public async Task<ActionResult<ApiResponse<WatchHistoryDto>>> AddToHistory(AddToWatchHistoryRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<WatchHistoryDto>.Fail("unauthorized"));
        }

        var entry = await _userDataService.AddToWatchHistoryAsync(userId.Value, request.MovieId, request.Progress, cancellationToken);
        var movie = await _movieMetadataClient.GetMovieAsync(entry.MovieId, cancellationToken);
        return Ok(ApiResponse<WatchHistoryDto>.Ok(MapHistoryEntry(entry, movie), "History updated"));
    }

    [HttpGet("favorites")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MovieDto>>>> GetFavorites(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<IEnumerable<MovieDto>>.Fail("unauthorized"));
        }

        var library = await _userDataService.GetLibraryAsync(userId.Value, cancellationToken);
        var favoriteIds = library.Where(x => x.IsFavorite).Select(x => x.MovieId);
        var movieMap = await _movieMetadataClient.GetMoviesAsync(favoriteIds, cancellationToken);
        return Ok(ApiResponse<IEnumerable<MovieDto>>.Ok(movieMap.Values));
    }

    [HttpGet("watch-later")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MovieDto>>>> GetWatchLater(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<IEnumerable<MovieDto>>.Fail("unauthorized"));
        }

        var watchLater = await _userDataService.GetWatchLaterAsync(userId.Value, cancellationToken);
        var movieMap = await _movieMetadataClient.GetMoviesAsync(watchLater.Select(x => x.MovieId), cancellationToken);
        var ordered = watchLater.Select(x => movieMap.TryGetValue(x.MovieId, out var movie) ? movie : CreatePlaceholderMovie(x.MovieId));
        return Ok(ApiResponse<IEnumerable<MovieDto>>.Ok(ordered));
    }

    [HttpPost("watch-later")]
    public async Task<ActionResult<ApiResponse<string?>>> AddToWatchLater(AddToWatchLaterRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<string?>.Fail("unauthorized"));
        }

        await _userDataService.AddToWatchLaterAsync(userId.Value, request.MovieId, cancellationToken);
        return Ok(ApiResponse<string?>.Ok(null, "Added to watch later"));
    }

    [HttpDelete("watch-later/{movieId:guid}")]
    public async Task<ActionResult<ApiResponse<string?>>> RemoveFromWatchLater(Guid movieId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<string?>.Fail("unauthorized"));
        }

        var removed = await _userDataService.RemoveFromWatchLaterAsync(userId.Value, movieId, cancellationToken);
        if (!removed)
        {
            return NotFound(ApiResponse<string?>.Fail("Movie not found in watch later"));
        }

        return Ok(ApiResponse<string?>.Ok(null, "Removed from watch later"));
    }

    private static UserProfileDto MapProfile(UserProfile profile) => new(
        profile.Id,
        profile.Email,
        profile.Username,
        profile.FirstName,
        profile.LastName,
        profile.Bio,
        profile.Role,
        profile.AvatarUrl,
        profile.CreatedAt,
        profile.UpdatedAt);

    private static LibraryItemDto MapLibraryItem(LibraryItem item, MovieDto? movie) => new(
        item.Id,
        item.MovieId,
        movie ?? CreatePlaceholderMovie(item.MovieId),
        item.UserId,
        item.Progress,
        item.LastWatched,
        item.IsFavorite,
        item.AddedAt);

    private static WatchHistoryDto MapHistoryEntry(WatchHistoryEntry entry, MovieDto? movie) => new(
        entry.Id,
        entry.MovieId,
        movie ?? CreatePlaceholderMovie(entry.MovieId),
        entry.UserId,
        entry.WatchedAt,
        entry.Progress,
        entry.Completed);

    private static MovieDto CreatePlaceholderMovie(Guid movieId) => new()
    {
        Id = movieId,
        Title = "Unavailable",
        Description = "Movie metadata is currently unavailable.",
        Poster = string.Empty,
        Year = DateTime.UtcNow.Year,
        Duration = 0,
        Rating = 0,
        Quality = "1080p",
        CreatedAt = DateTime.UtcNow,
        Views = 0
    };
}
