using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Extensions;
using TheKrystalShip.KGSM.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Integration;

/// <summary>
/// Integration tests for the <see cref="IInstanceService"/> with Necesse game instances.
/// </summary>
public class NecesseInstanceIntegrationTests : OutputTestBase, IClassFixture<NecesseTestInstanceFixture>
{
    private readonly NecesseTestInstanceFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="NecesseInstanceIntegrationTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    /// <param name="fixture">The test fixture providing a shared Necesse instance.</param>
    public NecesseInstanceIntegrationTests(ITestOutputHelper output, NecesseTestInstanceFixture fixture) : base(output)
    {
        _fixture = fixture;
    }

    [Fact]
    public void GetAll_ShouldReturnTestInstance()
    {
        // Arrange & Act
        var instances = KgsmClient.Instances.GetAll();

        // Assert
        instances.Should().NotBeNull();
        instances.Should().ContainKey(_fixture.InstanceName);
        
        // Log the instances for debugging
        Output.WriteLine($"Found {instances.Count} instances:");
        foreach (var instance in instances)
        {
            Output.WriteLine($"- {instance.Key}: {instance.Value.Blueprint}");
        }

        // Verify instance properties
        var testInstance = instances[_fixture.InstanceName];
        testInstance.Should().NotBeNull();
        testInstance.Blueprint.Should().Be("necesse");
        testInstance.Directory.Should().Be(_fixture.InstallDir);
    }

    [Fact]
    public void StartStop_ShouldChangeInstanceStatus()
    {
        // Arrange - ensure instance is stopped
        KgsmClient.Instances.Stop(_fixture.InstanceName);
        Thread.Sleep(1000); // Wait for stop to complete

        // Act & Assert - Start
        var startResult = KgsmClient.Instances.Start(_fixture.InstanceName);
        startResult.Should().NotBeNull();
        startResult.IsSuccess.Should().BeTrue();

        // Verify instance is running
        Thread.Sleep(2000); // Wait for start to complete
        var isActive = KgsmClient.Instances.IsActive(_fixture.InstanceName);
        isActive.Should().BeTrue("Instance should be active after starting");

        // Act & Assert - Stop
        var stopResult = KgsmClient.Instances.Stop(_fixture.InstanceName);
        stopResult.Should().NotBeNull();
        stopResult.IsSuccess.Should().BeTrue();

        // Verify instance is stopped
        Thread.Sleep(2000); // Wait for stop to complete
        isActive = KgsmClient.Instances.IsActive(_fixture.InstanceName);
        isActive.Should().BeFalse("Instance should be inactive after stopping");
    }

    [Fact]
    public void GetLogs_ShouldReturnLogContent()
    {
        // Arrange - ensure instance has been started at least once
        if (!KgsmClient.Instances.IsActive(_fixture.InstanceName))
        {
            KgsmClient.Instances.Start(_fixture.InstanceName);
            Thread.Sleep(2000); // Wait for start to generate logs
            KgsmClient.Instances.Stop(_fixture.InstanceName);
            Thread.Sleep(1000); // Wait for stop to complete
        }

        // Act
        var result = KgsmClient.Instances.GetLogs(_fixture.InstanceName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Stdout.Should().NotBeEmpty("Log file should contain content");
    }
}

/// <summary>
/// Test fixture that creates a Necesse instance for tests and cleans it up afterward.
/// </summary>
public class NecesseTestInstanceFixture : IDisposable
{
    public string InstanceName { get; }
    public string InstallDir { get; }

    private readonly IKgsmClient _kgsmClient;
    
    public NecesseTestInstanceFixture()
    {
        // Create a unique instance name and install directory
        InstanceName = $"necesse-test-{Guid.NewGuid().ToString()[..8]}";
        InstallDir = Path.Combine("/tmp/kgsm-test", InstanceName);
        Directory.CreateDirectory(InstallDir);

        // Create service provider for KGSM client
        var services = new ServiceCollection();
        services.AddKgsmServices("/home/heisen/kgsm/kgsm.sh", "/home/heisen/kgsm/kgsm.sock");

        var serviceProvider = services.BuildServiceProvider();
        _kgsmClient = serviceProvider.GetRequiredService<IKgsmClient>();

        // Install Necesse instance
        var result = _kgsmClient.Instances.Install("necesse", InstallDir, name: InstanceName);
        if (!result.IsSuccess)
        {
            throw new Exception($"Failed to install test Necesse instance: {result.Stderr}");
        }
    }

    public void Dispose()
    {
        try
        {
            // Stop instance if running
            _kgsmClient.Instances.Stop(InstanceName);
            
            // Uninstall test instance
            _kgsmClient.Instances.Uninstall(InstanceName);
            
            // Clean up test directory
            if (Directory.Exists(InstallDir))
            {
                Directory.Delete(InstallDir, true);
            }
        }
        catch (Exception)
        {
            // Ignore cleanup errors
        }
    }
}
