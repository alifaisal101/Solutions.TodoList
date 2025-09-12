namespace Solutions.TodoList.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection : ICollectionFixture<TodoListApiFixture>
{
    public const string Name = "IntegrationTests";
}
