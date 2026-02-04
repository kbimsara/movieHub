namespace UserService.Models.DTOs;

public record WatchHistoryDto
(
    Guid Id,
    Guid MovieId,
    MovieDto? Movie,
    Guid UserId,
    DateTime WatchedAt,
    double Progress,
    bool Completed
);
