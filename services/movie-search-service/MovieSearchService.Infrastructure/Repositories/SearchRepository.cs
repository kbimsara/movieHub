using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Options;
using MovieSearchService.Application.DTOs;
using MovieSearchService.Application.Interfaces;
using MovieSearchService.Domain.Entities;
using MovieSearchService.Infrastructure.Configuration;

namespace MovieSearchService.Infrastructure.Repositories;

/// <summary>
/// Elasticsearch implementation of search repository
/// Handles all Elasticsearch operations
/// </summary>
public class SearchRepository : ISearchRepository
{
    private readonly ElasticsearchClient _client;
    private readonly string _indexName;

    public SearchRepository(
        ElasticsearchClient client,
        IOptions<ElasticsearchSettings> settings)
    {
        _client = client;
        _indexName = settings.Value.IndexName;
    }

    public async Task<PagedResultDto<MovieSearchResultDto>> SearchMoviesAsync(MovieSearchRequestDto request)
    {
        var mustQueries = new List<Query>();

        // Full-text search on title and description
        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            mustQueries.Add(new MultiMatchQuery
            {
                Query = request.Q,
                Fields = new[] { "title^2", "description" }, // Boost title matches
                Type = TextQueryType.BestFields,
                Fuzziness = new Fuzziness("AUTO")
            });
        }

        // Genre filter (exact match)
        if (!string.IsNullOrWhiteSpace(request.Genre))
        {
            mustQueries.Add(new TermQuery("genre.keyword") 
            { 
                Field = "genre.keyword",
                Value = request.Genre 
            });
        }

        // Year filter (exact match)
        if (request.Year.HasValue)
        {
            mustQueries.Add(new TermQuery("releaseYear") 
            { 
                Field = "releaseYear",
                Value = request.Year.Value 
            });
        }

        // Calculate pagination
        var from = (request.Page - 1) * request.PageSize;

        var searchResponse = await _client.SearchAsync<SearchMovie>(s => s
            .Indices(_indexName)
            .Query(q =>
            {
                // If no queries, match all
                if (!mustQueries.Any())
                {
                    q.MatchAll(new MatchAllQuery());
                    return;
                }

                // Apply bool query with must conditions
                q.Bool(b => b.Must(mustQueries.ToArray()));
            })
            .From(from)
            .Size(request.PageSize)
            .Sort(sort => sort.Score(new ScoreSort { Order = SortOrder.Desc }))
        );

        if (!searchResponse.IsValidResponse)
        {
            throw new Exception($"Elasticsearch query failed: {searchResponse.ElasticsearchServerError?.Error?.Reason}");
        }

        var results = searchResponse.Documents.Select((doc, index) => new MovieSearchResultDto
        {
            Id = doc.Id,
            Title = doc.Title,
            Description = doc.Description,
            Genre = doc.Genre,
            ReleaseYear = doc.ReleaseYear,
            Rating = doc.Rating,
            CreatedAt = doc.CreatedAt,
            Score = searchResponse.Hits.ElementAtOrDefault(index)?.Score
        });

        return new PagedResultDto<MovieSearchResultDto>
        {
            Items = results,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = searchResponse.Total
        };
    }

    public async Task<bool> IndexMovieAsync(SearchMovie movie)
    {
        var response = await _client.IndexAsync(movie, idx => idx
            .Index(_indexName)
            .Id(movie.Id)
        );

        return response.IsValidResponse;
    }

    public async Task<bool> PingAsync()
    {
        var response = await _client.PingAsync();
        return response.IsValidResponse;
    }

    public async Task EnsureIndexExistsAsync()
    {
        var existsResponse = await _client.Indices.ExistsAsync(_indexName);

        if (existsResponse.Exists)
        {
            return;
        }

        // Create index - Elasticsearch will auto-map properties based on SearchMovie
        var createResponse = await _client.Indices.CreateAsync(_indexName);

        if (!createResponse.IsValidResponse)
        {
            throw new Exception($"Failed to create index: {createResponse.ElasticsearchServerError?.Error?.Reason}");
        }
    }
}
