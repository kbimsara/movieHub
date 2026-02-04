namespace UserService.Models.DTOs;

public record LibraryItemDto
(
    Guid Id,
    Guid MovieId,
    MovieDto? Movie,
    Guid UserId,
    double Progress,
    DateTime LastWatched,
    bool IsFavorite,
    DateTime AddedAt
);
