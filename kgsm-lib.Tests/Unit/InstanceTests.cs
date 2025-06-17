using FluentAssertions;
using TheKrystalShip.KGSM.Core.Models;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Instance"/> class.
/// </summary>
public class InstanceTests
{
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstanceTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    public InstanceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Instance_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var instance = new Instance();

        // Assert
        instance.Name.Should().BeEmpty();
        instance.LifecycleManager.Should().Be(LifecycleManager.Standalone);
        instance.Status.Should().Be(InstanceStatus.Inactive);
        instance.LogsDirectory.Should().BeEmpty();
        instance.Directory.Should().BeEmpty();
        instance.InstallationDate.Should().Be(DateTime.MinValue);
        instance.PID.Should().BeEmpty();
        instance.Version.Should().BeEmpty();
        instance.Blueprint.Should().BeEmpty();
        instance.ServiceFile.Should().BeEmpty();
        instance.SocketFile.Should().BeEmpty();
        instance.FirewallRule.Should().BeEmpty();
    }

    [Fact]
    public void Instance_ShouldInitializeWithProvidedValues()
    {
        // Arrange
        var now = DateTime.Now;

        // Act
        var instance = new Instance
        {
            Name = "factorio-server-1",
            LifecycleManager = LifecycleManager.Systemd,
            Status = InstanceStatus.Active,
            LogsDirectory = "/var/log/kgsm/factorio-server-1",
            Directory = "/opt/kgsm/factorio-server-1",
            InstallationDate = now,
            PID = "12345",
            Version = "1.1.87",
            Blueprint = "factorio",
            ServiceFile = "/etc/systemd/system/kgsm-factorio-server-1.service",
            SocketFile = "/var/run/kgsm-factorio-server-1.sock",
            FirewallRule = "kgsm-factorio-server-1"
        };

        // Assert
        instance.Name.Should().Be("factorio-server-1");
        instance.LifecycleManager.Should().Be(LifecycleManager.Systemd);
        instance.Status.Should().Be(InstanceStatus.Active);
        instance.LogsDirectory.Should().Be("/var/log/kgsm/factorio-server-1");
        instance.Directory.Should().Be("/opt/kgsm/factorio-server-1");
        instance.InstallationDate.Should().Be(now);
        instance.PID.Should().Be("12345");
        instance.Version.Should().Be("1.1.87");
        instance.Blueprint.Should().Be("factorio");
        instance.ServiceFile.Should().Be("/etc/systemd/system/kgsm-factorio-server-1.service");
        instance.SocketFile.Should().Be("/var/run/kgsm-factorio-server-1.sock");
        instance.FirewallRule.Should().Be("kgsm-factorio-server-1");
    }

    [Fact]
    public void Instance_ToString_ShouldContainAllProperties()
    {
        // Arrange
        var now = DateTime.Now;
        var instance = new Instance
        {
            Name = "factorio-server-1",
            LifecycleManager = LifecycleManager.Systemd,
            Status = InstanceStatus.Active,
            LogsDirectory = "/var/log/kgsm/factorio-server-1",
            Directory = "/opt/kgsm/factorio-server-1",
            InstallationDate = now,
            PID = "12345",
            Version = "1.1.87",
            Blueprint = "factorio",
            ServiceFile = "/etc/systemd/system/kgsm-factorio-server-1.service",
            SocketFile = "/var/run/kgsm-factorio-server-1.sock",
            FirewallRule = "kgsm-factorio-server-1"
        };

        // Act
        var result = instance.ToString();
        _output.WriteLine($"ToString result: {result}");

        // Assert
        result.Should().Contain("factorio-server-1");
        result.Should().Contain("Systemd");
        result.Should().Contain("Active");
        result.Should().Contain("/var/log/kgsm/factorio-server-1");
        result.Should().Contain("/opt/kgsm/factorio-server-1");
        result.Should().Contain(now.ToString());
        result.Should().Contain("12345");
        result.Should().Contain("1.1.87");
        result.Should().Contain("factorio");
        result.Should().Contain("/etc/systemd/system/kgsm-factorio-server-1.service");
        result.Should().Contain("/var/run/kgsm-factorio-server-1.sock");
        result.Should().Contain("kgsm-factorio-server-1");
    }
}
