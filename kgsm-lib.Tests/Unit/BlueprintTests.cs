using FluentAssertions;
using TheKrystalShip.KGSM.Core.Models;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Blueprint"/> class.
/// </summary>
public class BlueprintTests
{
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlueprintTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    public BlueprintTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Blueprint_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var blueprint = new Blueprint();

        // Assert
        blueprint.Name.Should().BeEmpty();
        blueprint.Ports.Should().BeEmpty();
        blueprint.SteamAppId.Should().BeEmpty();
        blueprint.IsSteamAccountRequired.Should().BeFalse();
        blueprint.ExecutableFile.Should().BeEmpty();
        blueprint.ExecutableSubdirectory.Should().BeEmpty();
        blueprint.ExecutableArguments.Should().BeEmpty();
        blueprint.LevelName.Should().BeEmpty();
        blueprint.StopCommand.Should().BeNull();
        blueprint.SaveCommand.Should().BeNull();
    }

    [Fact]
    public void Blueprint_ShouldInitializeWithProvidedValues()
    {
        // Arrange & Act
        var blueprint = new Blueprint
        {
            Name = "factorio",
            Ports = "34197",
            SteamAppId = "427520",
            IsSteamAccountRequired = true,
            ExecutableFile = "factorio",
            ExecutableSubdirectory = "bin/x64",
            ExecutableArguments = "--start-server",
            LevelName = "default",
            StopCommand = "/quit",
            SaveCommand = "/save"
        };

        // Assert
        blueprint.Name.Should().Be("factorio");
        blueprint.Ports.Should().Be("34197");
        blueprint.SteamAppId.Should().Be("427520");
        blueprint.IsSteamAccountRequired.Should().BeTrue();
        blueprint.ExecutableFile.Should().Be("factorio");
        blueprint.ExecutableSubdirectory.Should().Be("bin/x64");
        blueprint.ExecutableArguments.Should().Be("--start-server");
        blueprint.LevelName.Should().Be("default");
        blueprint.StopCommand.Should().Be("/quit");
        blueprint.SaveCommand.Should().Be("/save");
    }

    [Fact]
    public void Blueprint_ToString_ShouldContainAllProperties()
    {
        // Arrange
        var blueprint = new Blueprint
        {
            Name = "factorio",
            Ports = "34197",
            SteamAppId = "427520",
            IsSteamAccountRequired = true,
            ExecutableFile = "factorio",
            ExecutableSubdirectory = "bin/x64",
            ExecutableArguments = "--start-server",
            LevelName = "default",
            StopCommand = "/quit",
            SaveCommand = "/save"
        };

        // Act
        var result = blueprint.ToString();
        _output.WriteLine($"ToString result: {result}");

        // Assert
        result.Should().Contain("factorio");
        result.Should().Contain("34197");
        result.Should().Contain("427520");
        result.Should().Contain("True");
        result.Should().Contain("bin/x64");
        result.Should().Contain("--start-server");
        result.Should().Contain("default");
        result.Should().Contain("/quit");
        result.Should().Contain("/save");
    }
}
