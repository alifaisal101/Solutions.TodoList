using System.Text.Json.Serialization;
using Solutions.TodoList.Domain.Exceptions;

namespace Solutions.TodoList.Domain.ValueObjects;

[JsonConverter(typeof(TodoDescriptionJsonConverter))]
public sealed class TodoDescription : IEquatable<TodoDescription>
{
    public const int MaxLength = 1000;

    public string Value { get; private set; }

    private TodoDescription() => Value = string.Empty;

    public TodoDescription(string? value)
    {
        var trimmed = value?.Trim() ?? string.Empty;
        if (trimmed.Length > MaxLength)
            throw new DomainValidationException($"Description must not exceed {MaxLength} characters.");

        Value = trimmed;
    }

    public override string ToString() => Value;
    public bool Equals(TodoDescription? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as TodoDescription);
    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator string(TodoDescription description) => description.Value;
}
