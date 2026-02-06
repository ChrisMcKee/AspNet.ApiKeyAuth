using System.Security.Claims;
using System.Text.Encodings.Web;
using AspNet.ApiKeyAuth.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNet.ApiKeyAuth;

/// <summary>
/// Handles API key-based authentication by validating incoming requests against a specified API key.
/// </summary>
/// <remarks>
/// This class derives from <see cref="AuthenticationHandler{TOptions}"/> and implements custom authentication logic
/// for requests carrying API keys in their headers. If the API key in the request matches the pre-configured keys,
/// the handler authenticates the request and assigns claims to the authenticated user.
/// </remarks>
/// <example>
/// The handler can be configured in the ASP.NET Core pipeline via an authentication scheme
/// using <c>AddApiKey</c> defined in the <c>AuthenticationBuilderExtensions</c>.
/// </example>
/// <seealso cref="ApiKeyAuthenticationOptions"/>
/// <seealso cref="AuthenticationBuilderExtensions"/>
public sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IApiKeyCredentialsAccessor credentialsAccessor)
    : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var headerValue))
        {
            return AuthenticateResult.NoResult();
        }

        var apiKey = headerValue.ToString();

        var keyConfig = await credentialsAccessor.GetApiKeyAsync(apiKey, Context.RequestAborted);

        if (keyConfig is null)
        {
            return AuthenticateResult.Fail("Invalid API key.");
        }

        var claims = new List<Claim>(2)
        {
            new(ClaimTypes.Name, keyConfig.Name),
            new(ClaimTypes.NameIdentifier, keyConfig.Name),
        };

        foreach (var permission in keyConfig.Claims)
        {
            claims.Add(new Claim(ApiKeyAuthenticationDefaults.PermissionClaimType, permission));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
