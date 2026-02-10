using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNet.ApiKeyAuth.RateLimit;

public record ApiKeyRateLimitOptions
{
    public int PermitLimit { get; init; }
    public int QueueLimit { get; init; }
    public int Window { get; init; }
    public int SegmentsPerWindow { get; init; }
}

public class ApiKeyRateLimitPolicy : IRateLimiterPolicy<string>
{
    private Func<OnRejectedContext, CancellationToken, ValueTask>? _onRejected;
    private readonly ApiKeyRateLimitOptions _options;

    public ApiKeyRateLimitPolicy(ILogger<ApiKeyRateLimitPolicy> logger,
        IOptions<ApiKeyRateLimitOptions> options)
    {
        _onRejected = (ctx, token) =>
        {
            ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            logger.LogWarning($"Request rejected by {nameof(ApiKeyRateLimitPolicy)}");
            return ValueTask.CompletedTask;
        };
        _options = options.Value;
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => _onRejected;

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        string apiKey = httpContext.Request.Headers["X-API-Key"].ToString() ?? "no-key";
        return apiKey switch
        {
            _ => RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: apiKey,
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = _options.PermitLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = _options.QueueLimit,
                    Window = TimeSpan.FromSeconds(_options.Window),
                    SegmentsPerWindow = _options.SegmentsPerWindow
                }),
        };
    }
}

