using Microsoft.AspNetCore.Authentication;

namespace AspNet.ApiKeyAuth;

public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string HeaderName { get; set; } = ApiKeyAuthenticationDefaults.HeaderName;
}
