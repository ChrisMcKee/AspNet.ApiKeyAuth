namespace AspNet.ApiKeyAuth.Configuration;

public sealed class ApiKeyAuthenticationConfiguration
{
    public List<ApiKeyConfig> Keys { get; set; } = [];
}
