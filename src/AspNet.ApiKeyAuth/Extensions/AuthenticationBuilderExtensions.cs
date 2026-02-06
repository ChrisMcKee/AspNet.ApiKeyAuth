using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNet.ApiKeyAuth.Extensions;

public static class AuthenticationBuilderExtensions
{
    extension(AuthenticationBuilder builder)
    {
        public AuthenticationBuilder AddApiKey()
            => builder.AddApiKey(ApiKeyAuthenticationDefaults.AuthenticationScheme, _ => { });

        public AuthenticationBuilder AddApiKey(Action<ApiKeyAuthenticationOptions> configureOptions)
            => builder.AddApiKey(ApiKeyAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public AuthenticationBuilder AddApiKey(string authenticationScheme,
            Action<ApiKeyAuthenticationOptions> configureOptions)
        {
            builder.Services.TryAddSingleton<IApiKeyCredentialsAccessor, ApiKeyDefaultCredentialsAccessor>();

            builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                authenticationScheme, configureOptions);

            return builder;
        }
    }
}
