using Microsoft.AspNetCore.Mvc;

namespace MovieService.Models.Filters;

public class MovieFilter
{
    private const int MaxPageSize = 100;

    [FromQuery(Name = "q")]
    public string? Query { get; set; }

    [FromQuery(Name = "genres")]
    public List<string>? Genres { get; set; }

    [FromQuery(Name = "year")]
    public int? Year { get; set; }

    [FromQuery(Name = "quality")]
    public List<string>? Quality { get; set; }

    [FromQuery(Name = "rating")]
    public double? MinimumRating { get; set; }

    [FromQuery(Name = "sortBy")]
    public string? SortBy { get; set; }

    [FromQuery(Name = "sortOrder")]
    public string? SortOrder { get; set; } = "desc";

    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "pageSize")]
    public int PageSize { get; set; } = 20;

    public void Normalize()
    {
        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, MaxPageSize);
        SortOrder = string.Equals(SortOrder, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";
    }
}
