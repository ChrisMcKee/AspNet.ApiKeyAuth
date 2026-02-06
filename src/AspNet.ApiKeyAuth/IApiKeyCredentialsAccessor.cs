using AspNet.ApiKeyAuth.Configuration;

namespace AspNet.ApiKeyAuth;

/// <summary>
/// Defines a contract for retrieving API key credentials.
/// Implement this interface to load API keys from a custom data store (e.g., Redis, MongoDB).
/// </summary>
public interface IApiKeyCredentialsAccessor
{
    /// <summary>
    /// Retrieves the API key configuration for the specified key.
    /// </summary>
    /// <param name="apiKey">The API key to look up.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The matching <see cref="ApiKeyConfig"/> if found; otherwise, <c>null</c>.</returns>
    Task<ApiKeyConfig?> GetApiKeyAsync(string apiKey, CancellationToken cancellationToken);
}
