using FluentAssertions;
using System.Text.RegularExpressions;
using TheKrystalShip.KGSM.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Integration;

/// <summary>
/// Integration tests for the <see cref="IKgsmClient"/>.
/// </summary>
public class KgsmClientIntegrationTests : OutputTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KgsmClientIntegrationTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    public KgsmClientIntegrationTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void Help_ShouldReturnValidHelpText()
    {
        // Arrange & Act
        var result = KgsmClient.Help();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Stdout.Should().Contain("Usage:");
        result.Stdout.Should().Contain("Commands:");
    }

    [Fact]
    public void HelpInteractive_ShouldReturnValidInteractiveHelpText()
    {
        // Arrange & Act
        var result = KgsmClient.HelpInteractive();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Stdout.Should().Contain("Interactive mode commands:");
    }

    [Fact]
    public void GetVersion_ShouldReturnValidVersionInfo()
    {
        // Arrange & Act
        var result = KgsmClient.GetVersion();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        // Version should match semantic versioning pattern
        var versionPattern = new Regex(@"^\d+\.\d+\.\d+(?:-[a-zA-Z0-9]+)?$");
        versionPattern.IsMatch(result.Stdout.Trim()).Should().BeTrue();
    }

    [Fact]
    public void GetIp_ShouldReturnValidIpAddress()
    {
        // Arrange & Act
        var result = KgsmClient.GetIp();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        // IP should be in valid format (IPv4 or IPv6)
        var ipv4Pattern = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
        var ipv6Pattern = new Regex(@"^([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$");
        
        (ipv4Pattern.IsMatch(result.Stdout.Trim()) || ipv6Pattern.IsMatch(result.Stdout.Trim()))
            .Should().BeTrue($"Expected valid IP address but got: {result.Stdout.Trim()}");
    }

    [Fact]
    public void AdHoc_ShouldExecuteValidCommand()
    {
        // Arrange & Act
        var result = KgsmClient.AdHoc("--version");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        // Version should match semantic versioning pattern
        var versionPattern = new Regex(@"^\d+\.\d+\.\d+(?:-[a-zA-Z0-9]+)?$");
        versionPattern.IsMatch(result.Stdout.Trim()).Should().BeTrue();
    }
}
