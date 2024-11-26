using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonStringToBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString() ?? throw new InvalidOperationException("reader is null");
            return value == "1"; // Map "1" to true and anything else (like "0") to false
        }
        throw new JsonException("Invalid value for boolean conversion.");
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value ? "1" : "0");
    }
}
