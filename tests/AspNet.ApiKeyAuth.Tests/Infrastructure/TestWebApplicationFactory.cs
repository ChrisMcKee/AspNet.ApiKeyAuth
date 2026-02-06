using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace AspNet.ApiKeyAuth.Tests.Infrastructure;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApiKeyAuthentication:Keys:0:Key"] = "test-read-key",
                ["ApiKeyAuthentication:Keys:0:Name"] = "TestReader",
                ["ApiKeyAuthentication:Keys:0:Claims:0"] = "read",

                ["ApiKeyAuthentication:Keys:1:Key"] = "test-readwrite-key",
                ["ApiKeyAuthentication:Keys:1:Name"] = "TestReadWriter",
                ["ApiKeyAuthentication:Keys:1:Claims:0"] = "read",
                ["ApiKeyAuthentication:Keys:1:Claims:1"] = "write",
            });
        });
    }
}
