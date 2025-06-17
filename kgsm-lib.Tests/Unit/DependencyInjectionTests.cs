using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Extensions;
using TheKrystalShip.KGSM.Services;
using TheKrystalShip.KGSM.Tests.Common;
using Xunit;

namespace TheKrystalShip.KGSM.Tests.Unit;

/// <summary>
/// Unit tests for dependency injection and service resolution.
/// </summary>
public class DependencyInjectionTests
{

    [Fact]
    public void AddKgsmServices_ShouldRegisterAllDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddKgsmServices(TestConstants.KgsmPath, TestConstants.KgsmSocketPath);
        var serviceProvider = services.BuildServiceProvider();
        
        // Assert - All interfaces should resolve
        serviceProvider.GetService<IKgsmClient>().Should().NotBeNull();
        serviceProvider.GetService<IBlueprintService>().Should().NotBeNull();
        serviceProvider.GetService<IInstanceService>().Should().NotBeNull();
        serviceProvider.GetService<IEventService>().Should().NotBeNull();
        serviceProvider.GetService<IProcessRunner>().Should().NotBeNull();
        serviceProvider.GetService<IUnixSocketClient>().Should().NotBeNull();
    }

    [Fact]
    public void AddKgsmServices_ShouldRegisterCorrectImplementations()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddKgsmServices(TestConstants.KgsmPath, TestConstants.KgsmSocketPath);
        var serviceProvider = services.BuildServiceProvider();
        
        // Assert - All services should be of the correct type
        serviceProvider.GetService<IKgsmClient>().Should().BeOfType<KgsmClient>();
        serviceProvider.GetService<IBlueprintService>().Should().BeOfType<BlueprintService>();
        serviceProvider.GetService<IInstanceService>().Should().BeOfType<InstanceService>();
        serviceProvider.GetService<IEventService>().Should().BeOfType<EventService>();
        serviceProvider.GetService<IProcessRunner>().Should().BeOfType<ProcessRunner>();
        serviceProvider.GetService<IUnixSocketClient>().Should().BeOfType<UnixSocketClient>();
    }

    [Fact]
    public void KgsmClient_ShouldHaveCorrectDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKgsmServices(TestConstants.KgsmPath, TestConstants.KgsmSocketPath);
        
        // Act
        var serviceProvider = services.BuildServiceProvider();
        var kgsmClient = serviceProvider.GetRequiredService<IKgsmClient>();
        
        // Assert
        kgsmClient.Should().NotBeNull();
        kgsmClient.Blueprints.Should().NotBeNull();
        kgsmClient.Instances.Should().NotBeNull();
        kgsmClient.Events.Should().NotBeNull();
    }

    [Fact]
    public void AddKgsmServices_WithCustomOptions_ShouldRespectCustomConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        string customKgsmPath = "/custom/path/kgsm.sh";
        string customSocketPath = "/custom/path/kgsm.sock";
        
        // Act
        services.AddKgsmServices(customKgsmPath, customSocketPath);
        var serviceProvider = services.BuildServiceProvider();
        var processRunner = serviceProvider.GetRequiredService<IProcessRunner>();
        var socketClient = serviceProvider.GetRequiredService<IUnixSocketClient>();
        
        // Assert - Need to use reflection to check private fields
        var processRunnerType = processRunner.GetType();
        var processRunnerField = processRunnerType.GetField("_kgsmPath", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        var socketClientType = socketClient.GetType();
        var socketPathField = socketClientType.GetField("_socketPath", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        processRunnerField?.GetValue(processRunner).Should().Be(customKgsmPath);
        socketPathField?.GetValue(socketClient).Should().Be(customSocketPath);
    }
}
