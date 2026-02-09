using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNet.ApiKeyAuth.Extensions;

public static class AuthenticationBuilderExtensions
{
    /// <summary>
    /// Adds API key-based authentication to the authentication builder, using default settings.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> on which to configure API key authentication.</param>
    /// <returns>An updated <see cref="AuthenticationBuilder"/> with API key authentication configured.</returns>
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder)
        => builder.AddApiKey(ApiKeyAuthenticationDefaults.AuthenticationScheme, _ => { });

    /// <summary>
    /// Adds API key-based authentication to the authentication builder using the specified configuration.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> on which to configure API key authentication.</param>
    /// <param name="configureOptions">An <see cref="Action{T}"/> to configure the <see cref="ApiKeyAuthenticationOptions"/>.</param>
    /// <returns>An updated <see cref="AuthenticationBuilder"/> with API key authentication configured.</returns>
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, Action<ApiKeyAuthenticationOptions> configureOptions)
        => builder.AddApiKey(ApiKeyAuthenticationDefaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Adds API key-based authentication to the authentication builder with the specified authentication scheme and configuration options.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> on which to configure API key authentication.</param>
    /// <param name="authenticationScheme">The name of the authentication scheme to use.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="ApiKeyAuthenticationOptions"/> for the scheme.</param>
    /// <returns>An updated <see cref="AuthenticationBuilder"/> with API key authentication configured.</returns>
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme,
        Action<ApiKeyAuthenticationOptions> configureOptions)
    {
        builder.Services.TryAddSingleton<IApiKeyCredentialsAccessor, ApiKeyDefaultCredentialsAccessor>();

        builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            authenticationScheme, configureOptions);

        return builder;
    }
}
