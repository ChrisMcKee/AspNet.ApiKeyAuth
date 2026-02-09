# What is this library for

## Description

`AspNet.ApiKeyAuth` is a lightweight API key authentication and authorization library for ASP.NET Core. 
It adds an API key authentication scheme, validates keys from the request header, and exposes simple policies for read/write access based on claims.
You can customise the policies during setup and load keys from any source by implementing the credentials accessor interface.

## Details

- Adds an `ApiKey` authentication scheme backed by `ApiKeyAuthenticationHandler`.
- Looks up API keys via `IApiKeyCredentialsAccessor` (default implementation reads from configuration).
- Attaches a `permission` claim for each configured permission on the key.
- Includes ready-to-use authorization policies for `read`, `write`, and `read/write`.

## Nuget

Package: <https://www.nuget.org/packages/AspNet.ApiKeyAuth/>

## Quick Start

## Minimal hosting (Program.cs)

```csharp
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
```

## Configuration

The default credentials accessor reads keys from configuration under `ApiKeyAuthentication`.

```json
{
  "ApiKeyAuthentication": {
    "Keys": [
      {
        "Name": "read-client",
        "Key": "read-key",
        "Claims": ["read"]
      },
      {
        "Name": "admin-client",
        "Key": "admin-key",
        "Claims": ["read", "write"]
      }
    ]
  }
}
```

## Custom key lookup

If you want to load keys from a database or external store, implement `IApiKeyCredentialsAccessor` and register it in DI.

```csharp
builder.Services.AddSingleton<IApiKeyCredentialsAccessor, MyCredentialsAccessor>();
```

The API key is read from the `X-Api-Key` header by default. You can customize this via `ApiKeyAuthenticationOptions`.

```csharp
builder.Services
    .AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
    .AddApiKey(options =>
    {
        options.HeaderName = "X-Api-Key";
    });
```

There are working examples in:

- `src/AspNet.ApiKeyAuthExample`
- `tests/AspNet.ApiKeyAuth.Tests`
