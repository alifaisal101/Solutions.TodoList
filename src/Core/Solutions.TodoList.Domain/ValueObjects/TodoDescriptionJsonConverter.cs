using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solutions.TodoList.Domain.ValueObjects;

public sealed class TodoDescriptionJsonConverter : JsonConverter<TodoDescription>
{
    public override TodoDescription Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        new(reader.GetString());

    public override void Write(Utf8JsonWriter writer, TodoDescription value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
