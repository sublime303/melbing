using System.Text.Json;
using System.Text.Json.Serialization;

namespace Melbing.ShipLog.Models;

/// <summary>
/// Accepts JSON booleans as true/false, 0/1, or "0"/"1" (ESP32 / ArduinoJson style).
/// </summary>
public sealed class FlexibleBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number when reader.TryGetInt64(out var n) => n != 0,
            JsonTokenType.String when bool.TryParse(reader.GetString(), out var b) => b,
            JsonTokenType.String when int.TryParse(reader.GetString(), out var i) => i != 0,
            JsonTokenType.Null => false,
            _ => throw new JsonException($"Unexpected token {reader.TokenType} for boolean value."),
        };
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}

public sealed class FlexibleNullableBoolConverter : JsonConverter<bool?>
{
    private static readonly FlexibleBoolConverter Inner = new();

    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return Inner.Read(ref reader, typeof(bool), options);
    }

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteBooleanValue(value.Value);
    }
}
