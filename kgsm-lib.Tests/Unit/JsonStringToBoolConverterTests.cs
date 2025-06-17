using FluentAssertions;
using System.Text.Json;
using TheKrystalShip.KGSM;
using TheKrystalShip.KGSM.Services;
using Xunit;

namespace TheKrystalShip.KGSM.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="JsonStringToBoolConverter"/>.
/// </summary>
public class JsonStringToBoolConverterTests
{
    private readonly JsonStringToBoolConverter _converter = new();

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("True", true)]
    [InlineData("False", false)]
    [InlineData("TRUE", true)]
    [InlineData("FALSE", false)]
    public void Read_WithValidBoolString_ShouldConvertCorrectly(string jsonValue, bool expected)
    {
        // Arrange
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes($"\"{jsonValue}\""));
        reader.Read(); // Advance reader

        // Act
        var result = _converter.Read(ref reader, typeof(bool), new JsonSerializerOptions());

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Write_WithBoolValue_ShouldWriteCorrectJsonString(bool value, string expected)
    {
        // Arrange
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act
        _converter.Write(writer, value, new JsonSerializerOptions());
        writer.Flush();
        var json = System.Text.Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        json.Should().Be($"\"{expected}\"");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("0")]
    [InlineData("yes")]
    [InlineData("no")]
    [InlineData("invalid")]
    [InlineData("")]
    public void Read_WithInvalidBoolString_ShouldThrowException(string jsonValue)
    {
        // Act & Assert - Need to directly call to avoid ref issues
        Assert.Throws<JsonException>(() => 
        {
            // Arrange
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes($"\"{jsonValue}\""));
            reader.Read(); // Advance reader
            _converter.Read(ref reader, typeof(bool), new JsonSerializerOptions()); 
        });
    }
}
