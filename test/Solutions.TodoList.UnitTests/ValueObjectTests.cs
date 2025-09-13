using FluentAssertions;
using Solutions.TodoList.Domain.Exceptions;
using Solutions.TodoList.Domain.ValueObjects;

namespace Solutions.TodoList.UnitTests;

public class ValueObjectTests
{
    [Fact]
    public void TodoTitle_trims_whitespace()
    {
        new TodoTitle("  Buy milk  ").Value.Should().Be("Buy milk");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void TodoTitle_rejects_blank(string value)
    {
        var act = () => new TodoTitle(value);
        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void TodoTitle_rejects_value_over_max_length()
    {
        var act = () => new TodoTitle(new string('x', TodoTitle.MaxLength + 1));
        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void TodoTitle_equality_is_by_value()
    {
        new TodoTitle("same").Should().Be(new TodoTitle("same"));
    }

    [Fact]
    public void TodoDescription_treats_null_as_empty()
    {
        new TodoDescription(null).Value.Should().BeEmpty();
    }

    [Fact]
    public void TodoDescription_rejects_value_over_max_length()
    {
        var act = () => new TodoDescription(new string('x', TodoDescription.MaxLength + 1));
        act.Should().Throw<DomainValidationException>();
    }
}
