using System.Net;
using AspNet.ApiKeyAuth.Tests.Infrastructure;

namespace AspNet.ApiKeyAuth.Tests;

public class ApiKeyAuthenticationTests(TestWebApplicationFactory factory)
    : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PublicEndpoint_NoKey_Returns200()
    {
        var response = await _client.GetAsync("/public");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetData_ValidReadKey_Returns200()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/data");
        request.Headers.Add("X-Api-Key", "test-read-key");

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetData_ValidReadWriteKey_Returns200()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/data");
        request.Headers.Add("X-Api-Key", "test-readwrite-key");

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetData_NoKey_Returns401()
    {
        var response = await _client.GetAsync("/data");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetData_InvalidKey_Returns401()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/data");
        request.Headers.Add("X-Api-Key", "bogus-key");

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostData_ReadOnlyKey_Returns403()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/data");
        request.Headers.Add("X-Api-Key", "test-read-key");

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PostData_ReadWriteKey_Returns201()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/data");
        request.Headers.Add("X-Api-Key", "test-readwrite-key");

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetAdmin_ReadOnlyKey_Returns403()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/admin");
        request.Headers.Add("X-Api-Key", "test-read-key");

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAdmin_ReadWriteKey_Returns200()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/admin");
        request.Headers.Add("X-Api-Key", "test-readwrite-key");

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
