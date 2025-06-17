using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheKrystalShip.KGSM;

/// <summary>
/// Converts a JSON string value to a boolean.
/// Used to handle cases where boolean values are represented as strings like "0" and "1".
/// </summary>
public class JsonStringToBoolConverter : JsonConverter<bool>
{
    /// <summary>
    /// Reads and converts the JSON to type bool.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? value = reader.GetString();
            return value == "1"; // Map "1" to true and anything else (like "0") to false
        }
        throw new JsonException("Invalid value for boolean conversion.");
    }

    /// <summary>
    /// Writes the boolean value as a JSON string.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value ? "1" : "0");
    }
}
