using FluentAssertions;
using TheKrystalShip.KGSM.Core.Models;
using TheKrystalShip.KGSM.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Integration;

/// <summary>
/// Integration tests for the <see cref="IBlueprintService"/>.
/// </summary>
public class BlueprintIntegrationTests : OutputTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BlueprintIntegrationTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    public BlueprintIntegrationTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void GetAll_ShouldReturnBlueprintsIncludingGameBlueprints()
    {
        // Arrange & Act
        var blueprints = KgsmClient.Blueprints.GetAll();

        // Assert
        blueprints.Should().NotBeNull();
        blueprints.Should().NotBeEmpty();
        
        // Log the blueprints for debugging
        Output.WriteLine($"Found {blueprints.Count} blueprints:");
        foreach (var blueprint in blueprints)
        {
            Output.WriteLine($"- {blueprint.Key}: {blueprint.Value.Name}");
        }
        
        // Verify our test blueprints are available
        blueprints.Keys.Should().Contain("factorio");
        blueprints.Keys.Should().Contain("necesse");
        blueprints.Keys.Should().Contain("terraria");
        
        // Verify the factorio blueprint properties
        var factorio = blueprints["factorio"];
        factorio.Should().NotBeNull();
        factorio.Name.Should().Be("factorio");
        factorio.Ports.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldCreateAndReturnNewBlueprint()
    {
        // Arrange
        string uniqueName = $"test-blueprint-{Guid.NewGuid().ToString()[..8]}";
        var blueprint = new Blueprint
        {
            Name = uniqueName,
            Ports = "34197/udp",
            ExecutableFile = "factorio",
            ExecutableSubdirectory = "bin/x64",
            ExecutableArguments = "--start-server-load-latest --server-settings ./data/server-settings.json"
        };

        try
        {
            // Act
            var result = KgsmClient.Blueprints.Create(blueprint);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Stdout.Should().Contain(uniqueName);

            // Verify the blueprint was created
            var blueprints = KgsmClient.Blueprints.GetAll();
            blueprints.Should().ContainKey(uniqueName);
        }
        finally
        {
            // Cleanup - remove the test blueprint
            KgsmClient.AdHoc("blueprint", "remove", uniqueName);
        }
    }
}
