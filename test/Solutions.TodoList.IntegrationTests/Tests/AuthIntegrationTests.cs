using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Solutions.TodoList.IntegrationTests.Infrastructure;

namespace Solutions.TodoList.IntegrationTests.Tests;

[Collection(IntegrationTestCollection.Name)]
public sealed class AuthIntegrationTests(TodoListApiFixture fixture)
{
    [Fact]
    public async Task Register_returns_access_token_and_sets_httponly_refresh_cookie()
    {
        var client = fixture.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            username = $"reg-{Guid.NewGuid():N}",
            password = "Sup3rSecret!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<TodoListApiFixture.AuthResponseDto>();
        body!.AccessToken.Should().NotBeNullOrWhiteSpace();

        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        var refreshCookie = cookies!.Single(c => c.StartsWith("refreshToken="));
        refreshCookie.Should().ContainAll("httponly", "secure", "samesite=strict");
    }

    [Fact]
    public async Task Login_with_valid_credentials_succeeds()
    {
        var client = fixture.CreateClient();
        var username = $"login-{Guid.NewGuid():N}";
        const string password = "Sup3rSecret!";

        await client.PostAsJsonAsync("/api/v1/auth/register", new { username, password });

        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new { username, password });

        login.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await login.Content.ReadFromJsonAsync<TodoListApiFixture.AuthResponseDto>();
        body!.Username.Should().Be(username);
    }

    [Fact]
    public async Task Login_with_wrong_password_is_unauthorized()
    {
        var client = fixture.CreateClient();
        var username = $"wrong-{Guid.NewGuid():N}";
        await client.PostAsJsonAsync("/api/v1/auth/register", new { username, password = "Sup3rSecret!" });

        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new { username, password = "nope" });

        login.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
