using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace AspNet.ApiKeyAuth.Extensions;

public static class AuthorizationExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds API key authorization policies to the service collection.
        /// Configures policies for "read", "write", and "read/write" access
        /// based on claims in the API key authentication scheme.
        /// </summary>
        /// <returns>
        /// The updated IServiceCollection instance with API key authorisation configured.
        /// </returns>
        public IServiceCollection AddApiKeyAuthorization()
        {
            services.AddAuthorizationBuilder()
                    .AddPolicy(ApiKeyAuthenticationDefaults.ReadPolicy, policy => policy.RequireClaim(ApiKeyAuthenticationDefaults.PermissionClaimType, "read"))
                    .AddPolicy(ApiKeyAuthenticationDefaults.WritePolicy, policy => policy.RequireClaim(ApiKeyAuthenticationDefaults.PermissionClaimType, "write"))
                    .AddPolicy(ApiKeyAuthenticationDefaults.ReadWritePolicy, policy =>
                        policy
                            .RequireClaim(ApiKeyAuthenticationDefaults.PermissionClaimType, "read")
                            .RequireClaim(ApiKeyAuthenticationDefaults.PermissionClaimType, "write"));

            return services;
        }

        /// <summary>
        /// Adds API key authorisation policies to the service collection.
        /// Configures custom policies defined in the provided dictionary based on the API key authentication scheme.
        /// </summary>
        /// <param name="policies">
        /// A dictionary containing policy names as keys and actions to configure each policy as values.
        /// </param>
        /// <returns>
        /// The updated IServiceCollection instance with the specified API key authorisation policies configured.
        /// </returns>
        public IServiceCollection AddApiKeyAuthorization(IReadOnlyDictionary<string, Action<AuthorizationPolicyBuilder>> policies)
        {
            var builder = services.AddAuthorizationBuilder();

            foreach (var policy in policies)
            {
                builder.AddPolicy(policy.Key, policy.Value);
            }

            return services;
        }
    }
}
