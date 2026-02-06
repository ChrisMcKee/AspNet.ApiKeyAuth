namespace AspNet.ApiKeyAuth.Configuration;

public sealed class ApiKeyConfig
{
    public required string Key { get; set; }
    public required string Name { get; set; }
    public List<string> Claims { get; set; } = [];
}
