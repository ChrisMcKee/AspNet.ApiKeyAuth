using AspNet.ApiKeyAuth;
using AspNet.ApiKeyAuth.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
    .AddApiKey();

builder.Services.AddApiKeyAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/public", () => "Hello, world!");

app.MapGet("/data", () => Results.Ok("read data"))
    .RequireAuthorization(ApiKeyAuthenticationDefaults.ReadPolicy);

app.MapPost("/data", () => Results.Created("/data/1", "created"))
    .RequireAuthorization(ApiKeyAuthenticationDefaults.WritePolicy);

app.MapGet("/admin", () => Results.Ok("admin access"))
    .RequireAuthorization(ApiKeyAuthenticationDefaults.ReadWritePolicy);

app.Run();

public partial class Program { }
