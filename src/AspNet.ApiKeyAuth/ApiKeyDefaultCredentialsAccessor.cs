using AspNet.ApiKeyAuth.Configuration;
using Microsoft.Extensions.Configuration;

namespace AspNet.ApiKeyAuth;

/// <summary>
/// Default implementation of <see cref="IApiKeyCredentialsAccessor"/> that loads API keys
/// from the application configuration section specified by
/// <see cref="ApiKeyAuthenticationDefaults.ConfigurationSection"/>.
/// </summary>
public sealed class ApiKeyDefaultCredentialsAccessor : IApiKeyCredentialsAccessor
{
    private readonly List<ApiKeyConfig> _keys;

    public ApiKeyDefaultCredentialsAccessor(IConfiguration configuration)
    {
        var section = configuration.GetSection(ApiKeyAuthenticationDefaults.ConfigurationSection);
        var config = new ApiKeyAuthenticationConfiguration();
        section.Bind(config);
        _keys = config.Keys;
    }

    public Task<ApiKeyConfig?> GetApiKeyAsync(string apiKey, CancellationToken cancellationToken)
    {
        var keyConfig = _keys.Find(k =>
            string.Equals(k.Key, apiKey, StringComparison.Ordinal));

        return Task.FromResult(keyConfig);
    }
}
