namespace AspNet.ApiKeyAuth;

public static class ApiKeyAuthenticationDefaults
{
    public const string AuthenticationScheme = "ApiKey";
    public const string HeaderName = "X-Api-Key";
    public const string PermissionClaimType = "permission";
    public const string ConfigurationSection = "ApiKeyAuthentication";

    public const string ReadPolicy = "ApiKeyRead";
    public const string WritePolicy = "ApiKeyWrite";
    public const string ReadWritePolicy = "ApiKeyReadWrite";
}
