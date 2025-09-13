using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
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

    private readonly Dictionary<string, string?> _settings = new()
    {
        ["Jwt__Issuer"] = "todo-api",
        ["Jwt__Audience"] = "todo-api-client",
        ["Jwt__AccessTokenMinutes"] = "30",
        ["Jwt__RefreshTokenDays"] = "7",
        ["Jwt__UseRsa"] = "false",
        ["Jwt__SymmetricKey"] = "integration-tests-symmetric-signing-key-please-change",
        ["Argon2__Iterations"] = "1",
        ["Argon2__MemoryKb"] = "8192",
        ["Argon2__DegreeOfParallelism"] = "1",
        ["Argon2__SaltSize"] = "16",
        ["Argon2__HashSize"] = "32"
    };

    public WebApplicationFactory<Program> Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        // The app reads configuration in Startup.ConfigureServices (invoked from Program before
        // builder.Build()), so the settings must exist in the default sources. Environment variables
        // are read by WebApplication.CreateBuilder immediately, unlike ConfigureAppConfiguration.
        _settings["ConnectionStrings__DefaultConnection"] = _postgres.GetConnectionString();
        foreach (var (key, value) in _settings)
            Environment.SetEnvironmentVariable(key, value);

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("IntegrationTests"));

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

    public async Task DisposeAsync()
    {
        foreach (var key in _settings.Keys)
            Environment.SetEnvironmentVariable(key, null);

        await Factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    public sealed record AuthResponseDto(Guid Id, string Username, string Role, string AccessToken);
}
