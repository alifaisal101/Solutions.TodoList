using System.Text.Json.Serialization;
using Solutions.TodoList.Domain.Exceptions;

namespace Solutions.TodoList.Domain.ValueObjects;

[JsonConverter(typeof(TodoTitleJsonConverter))]
public sealed class TodoTitle : IEquatable<TodoTitle>
{
    public const int MaxLength = 200;

    public string Value { get; private set; }

    private TodoTitle() => Value = string.Empty;

    public TodoTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException("Title is required.");

        var trimmed = value.Trim();
        if (trimmed.Length > MaxLength)
            throw new DomainValidationException($"Title must not exceed {MaxLength} characters.");

        Value = trimmed;
    }

    public override string ToString() => Value;
    public bool Equals(TodoTitle? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as TodoTitle);
    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator string(TodoTitle title) => title.Value;
}
