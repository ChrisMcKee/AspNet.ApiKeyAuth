using System.Globalization;
using System.Threading.RateLimiting;
using AspNet.ApiKeyAuth;
using AspNet.ApiKeyAuth.Extensions;
using AspNet.ApiKeyAuth.RateLimit;
using Microsoft.AspNetCore.RateLimiting;

//https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-10.0#token

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
    .AddApiKey();

builder.Services.AddApiKeyAuthorization();
const string apiRateLimitKey = "api";
builder.Services.AddRateLimiter(_=>
{
    _.OnRejected = async (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = ((int) retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
    };

    _.AddPolicy<string, ApiKeyRateLimitPolicy>(apiRateLimitKey);
});


var app = builder.Build();

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapGet("/public", () => "Hello, world!");

app.MapGet("/data", () => Results.Ok("read data"))
    .RequireAuthorization(ApiKeyAuthenticationDefaults.ReadPolicy)
    .RequireRateLimiting(apiRateLimitKey);

app.MapPost("/data", () => Results.Created("/data/1", "created"))
    .RequireAuthorization(ApiKeyAuthenticationDefaults.WritePolicy)
    .RequireRateLimiting(apiRateLimitKey);

app.MapGet("/admin", () => Results.Ok("admin access"))
    .RequireAuthorization(ApiKeyAuthenticationDefaults.ReadWritePolicy)
    .RequireRateLimiting(apiRateLimitKey);

app.Run();

public partial class Program { }
