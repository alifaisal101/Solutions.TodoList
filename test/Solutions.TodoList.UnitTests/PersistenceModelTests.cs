using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Solutions.TodoList.Domain.Entities;
using Solutions.TodoList.Infrastructure.Persistence;

namespace Solutions.TodoList.UnitTests;

public class PersistenceModelTests
{
    private static DatabaseContext BuildContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseNpgsql("Host=localhost;Database=model-only")
            .Options;
        return new DatabaseContext(options);
    }

    [Fact]
    public void Todo_maps_title_and_description_value_objects_as_complex_properties()
    {
        using var context = BuildContext();

        var todo = context.Model.FindEntityType(typeof(Todo));
        todo.Should().NotBeNull();

        var complexNames = todo!.GetComplexProperties().Select(p => p.Name);
        complexNames.Should().Contain([nameof(Todo.Title), nameof(Todo.Description)]);
    }

    [Fact]
    public void Todo_value_objects_persist_to_the_original_columns()
    {
        using var context = BuildContext();

        var todo = context.Model.FindEntityType(typeof(Todo))!;
        var titleColumn = todo.GetComplexProperties()
            .Single(p => p.Name == nameof(Todo.Title))
            .ComplexType.GetProperties()
            .Single(p => p.Name == "Value")
            .GetColumnName();

        titleColumn.Should().Be("Title");
    }
}
