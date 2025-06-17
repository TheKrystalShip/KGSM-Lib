using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Extensions;
using TheKrystalShip.KGSM.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Integration;

/// <summary>
/// Integration tests for the <see cref="IInstanceService"/> with Terraria game instances.
/// </summary>
public class TerrariaInstanceIntegrationTests : OutputTestBase, IClassFixture<TerrariaTestInstanceFixture>
{
    private readonly TerrariaTestInstanceFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerrariaInstanceIntegrationTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    /// <param name="fixture">The test fixture providing a shared Terraria instance.</param>
    public TerrariaInstanceIntegrationTests(ITestOutputHelper output, TerrariaTestInstanceFixture fixture) : base(output)
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
        testInstance.Blueprint.Should().Be("terraria");
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
    public void Backup_ShouldCreateBackup()
    {
        // Act
        var result = KgsmClient.AdHoc("backup", _fixture.InstanceName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Stdout.Should().Contain("Backup created");
        
        // Verify backup file exists in the output
        Output.WriteLine($"Backup result: {result.Stdout}");
    }
    
    [Fact]
    public void GetInstalledVersion_ShouldReturnVersion()
    {
        // Act
        var result = KgsmClient.Instances.GetInstalledVersion(_fixture.InstanceName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Stdout.Should().NotBeEmpty("Should return the installed version");
        
        Output.WriteLine($"Installed version: {result.Stdout}");
    }
    
    [Fact]
    public void GetLatestVersion_ShouldReturnAvailableVersion()
    {
        // Act
        var result = KgsmClient.Instances.GetLatestVersion(_fixture.InstanceName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Stdout.Should().NotBeEmpty("Should return the latest available version");
        
        Output.WriteLine($"Latest version: {result.Stdout}");
    }
}

/// <summary>
/// Test fixture that creates a Terraria instance for tests and cleans it up afterward.
/// </summary>
public class TerrariaTestInstanceFixture : IDisposable
{
    public string InstanceName { get; }
    public string InstallDir { get; }

    private readonly IKgsmClient _kgsmClient;
    
    public TerrariaTestInstanceFixture()
    {
        // Create a unique instance name and install directory
        InstanceName = $"terraria-test-{Guid.NewGuid().ToString()[..8]}";
        InstallDir = Path.Combine("/tmp/kgsm-test", InstanceName);
        Directory.CreateDirectory(InstallDir);

        // Create service provider for KGSM client
        var services = new ServiceCollection();
        services.AddKgsmServices("/home/heisen/kgsm/kgsm.sh", "/home/heisen/kgsm/kgsm.sock");

        var serviceProvider = services.BuildServiceProvider();
        _kgsmClient = serviceProvider.GetRequiredService<IKgsmClient>();

        // Install Terraria instance
        var result = _kgsmClient.Instances.Install("terraria", InstallDir, name: InstanceName);
        if (!result.IsSuccess)
        {
            throw new Exception($"Failed to install test Terraria instance: {result.Stderr}");
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
