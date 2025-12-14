using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace MovieHub.Shared.Kernel.API.Middleware;

/// <summary>
/// Rate limiting middleware
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly int _requestLimit;
    private readonly TimeSpan _timeWindow;

    public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache, int requestLimit = 100, int timeWindowSeconds = 60)
    {
        _next = next;
        _cache = cache;
        _requestLimit = requestLimit;
        _timeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var cacheKey = $"rate_limit_{clientId}";

        if (!_cache.TryGetValue(cacheKey, out int requestCount))
        {
            requestCount = 0;
        }

        requestCount++;

        if (requestCount > _requestLimit)
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                error = "Rate limit exceeded. Please try again later."
            });
            return;
        }

        _cache.Set(cacheKey, requestCount, _timeWindow);

        await _next(context);
    }
}
