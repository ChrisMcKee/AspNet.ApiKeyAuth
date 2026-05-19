using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNet.ApiKeyAuth.RateLimit;

public record ApiKeyRateLimitOptions
{
    public const string SectionName = "ApiKeyRateLimit";

    public int PermitLimit { get; init; } = 100;
    public int QueueLimit { get; init; }
    public int Window { get; init; } = 60;
    public int SegmentsPerWindow { get; init; } = 6;
}

public class ApiKeyRateLimitPolicy : IRateLimiterPolicy<string>
{
    private readonly ApiKeyRateLimitOptions _options;

    public ApiKeyRateLimitPolicy(ILogger<ApiKeyRateLimitPolicy> logger,
        IOptions<ApiKeyRateLimitOptions> options)
    {
        OnRejected = (ctx, _) =>
        {
            ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            logger.LogWarning($"Request rejected by {nameof(ApiKeyRateLimitPolicy)}");
            return ValueTask.CompletedTask;
        };
        _options = Validate(options.Value);
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; }

    private static ApiKeyRateLimitOptions Validate(ApiKeyRateLimitOptions options)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(options.PermitLimit, 1, nameof(options.PermitLimit));
        ArgumentOutOfRangeException.ThrowIfLessThan(options.QueueLimit, 0, nameof(options.QueueLimit));
        ArgumentOutOfRangeException.ThrowIfLessThan(options.Window, 1, nameof(options.Window));
        ArgumentOutOfRangeException.ThrowIfLessThan(options.SegmentsPerWindow, 1, nameof(options.SegmentsPerWindow));

        return options;
    }

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        string apiKey = httpContext.Request.Headers["X-API-Key"].ToString();

        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: apiKey,
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = _options.PermitLimit,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = _options.QueueLimit,
                Window = TimeSpan.FromSeconds(_options.Window),
                SegmentsPerWindow = _options.SegmentsPerWindow
            });
    }
}

