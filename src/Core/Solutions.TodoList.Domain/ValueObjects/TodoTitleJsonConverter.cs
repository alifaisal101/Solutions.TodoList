using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solutions.TodoList.Domain.ValueObjects;

public sealed class TodoTitleJsonConverter : JsonConverter<TodoTitle>
{
    public override TodoTitle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        new(reader.GetString() ?? string.Empty);

    public override void Write(Utf8JsonWriter writer, TodoTitle value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
