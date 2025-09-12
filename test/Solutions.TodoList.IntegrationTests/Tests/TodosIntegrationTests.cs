using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Solutions.TodoList.IntegrationTests.Infrastructure;

namespace Solutions.TodoList.IntegrationTests.Tests;

[Collection(IntegrationTestCollection.Name)]
public sealed class TodosIntegrationTests(TodoListApiFixture fixture)
{
    [Fact]
    public async Task Create_then_get_returns_the_created_todo()
    {
        var client = await fixture.CreateAuthenticatedClientAsync();

        var create = await client.PostAsJsonAsync("/api/v1/todos", new { title = "Buy milk", description = "2%" });
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var id = (await create.Content.ReadFromJsonAsync<JsonElement>())
            .GetProperty("data").GetProperty("id").GetGuid();

        var get = await client.GetAsync($"/api/v1/todos/{id}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);

        var title = (await get.Content.ReadFromJsonAsync<JsonElement>())
            .GetProperty("data").GetProperty("title").GetString();
        title.Should().Be("Buy milk");
    }

    [Fact]
    public async Task Create_with_blank_title_returns_problem_details_400()
    {
        var client = await fixture.CreateAuthenticatedClientAsync();

        var create = await client.PostAsJsonAsync("/api/v1/todos", new { title = "   ", description = "x" });

        create.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        create.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
    }

    [Fact]
    public async Task Listing_is_scoped_to_the_authenticated_user()
    {
        var alice = await fixture.CreateAuthenticatedClientAsync();
        var bob = await fixture.CreateAuthenticatedClientAsync();

        await alice.PostAsJsonAsync("/api/v1/todos", new { title = "Alice task", description = "" });

        var bobList = await bob.GetAsync("/api/v1/todos");
        bobList.StatusCode.Should().Be(HttpStatusCode.OK);

        var total = (await bobList.Content.ReadFromJsonAsync<JsonElement>())
            .GetProperty("data").GetProperty("totalCount").GetInt32();
        total.Should().Be(0);
    }

    [Fact]
    public async Task Anonymous_request_is_unauthorized()
    {
        var client = fixture.CreateClient();

        var response = await client.GetAsync("/api/v1/todos");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
