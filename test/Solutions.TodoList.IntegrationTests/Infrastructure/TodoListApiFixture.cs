using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace Solutions.TodoList.IntegrationTests.Infrastructure;

public sealed class TodoListApiFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("todolist_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public WebApplicationFactory<Program> Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("IntegrationTests");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString(),
                    ["Jwt:Issuer"] = "todo-api",
                    ["Jwt:Audience"] = "todo-api-client",
                    ["Jwt:AccessTokenMinutes"] = "30",
                    ["Jwt:RefreshTokenDays"] = "7",
                    ["Jwt:UseRsa"] = "false",
                    ["Jwt:SymmetricKey"] = "integration-tests-symmetric-signing-key-please-change",
                    ["Argon2:Iterations"] = "1",
                    ["Argon2:MemoryKb"] = "8192",
                    ["Argon2:DegreeOfParallelism"] = "1",
                    ["Argon2:SaltSize"] = "16",
                    ["Argon2:HashSize"] = "32"
                });
            });
        });

        // Forces the host to boot, which applies EF migrations against the container.
        _ = Factory.CreateClient();
    }

    public HttpClient CreateClient() => Factory.CreateClient();

    public async Task<HttpClient> CreateAuthenticatedClientAsync(string? username = null)
    {
        var client = Factory.CreateClient();
        var register = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            username = username ?? $"user-{Guid.NewGuid():N}",
            password = "Sup3rSecret!"
        });
        register.EnsureSuccessStatusCode();

        var auth = await register.Content.ReadFromJsonAsync<AuthResponseDto>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);
        return client;
    }

    public Task DisposeAsync() => Task.WhenAll(Factory.DisposeAsync().AsTask(), _postgres.DisposeAsync().AsTask());

    public sealed record AuthResponseDto(Guid Id, string Username, string Role, string AccessToken);
}
