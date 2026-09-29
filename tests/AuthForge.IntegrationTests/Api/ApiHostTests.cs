using AuthForge.IntegrationTests.Infrastructure;

namespace AuthForge.IntegrationTests.Api;

[Collection("Postgres")]
public class ApiHostTests
{
    private readonly PostgresFixture _fixture;

    public ApiHostTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ApiHost_StartsSuccessfully()
    {
        await using var factory =
            new AuthForgeWebApplicationFactory(
                _fixture.ConnectionString);

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.EnsureSuccessStatusCode();
    }
}