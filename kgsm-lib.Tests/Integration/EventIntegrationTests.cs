using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Events;
using TheKrystalShip.KGSM.Tests.Common;
using TheKrystalShip.KGSM.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Integration;

/// <summary>
/// Integration tests for the <see cref="IEventService"/> with real KGSM events.
/// </summary>
public class EventIntegrationTests : OutputTestBase, IClassFixture<EventTestInstanceFixture>
{
    private readonly EventTestInstanceFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventIntegrationTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    /// <param name="fixture">The test fixture providing a shared test instance.</param>
    public EventIntegrationTests(ITestOutputHelper output, EventTestInstanceFixture fixture) : base(output)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RegisterHandler_ShouldReceiveInstanceStatusEvents()
    {
        // Arrange
        string instanceName = _fixture.InstanceName;
        var eventReceived = new TaskCompletionSource<bool>();
        
        // Initialize the event service
        KgsmClient.Events.Initialize();
        
        // Register handler for instance started events
        KgsmClient.Events.RegisterHandler<InstanceStartedData>(evt => 
        {
            Output.WriteLine($"Instance started event received for: {evt.InstanceName}");
            
            if (evt.InstanceName == instanceName)
            {
                eventReceived.TrySetResult(true);
            }
            
            return Task.CompletedTask;
        });

        // Trigger events by starting the instance
        KgsmClient.Instances.Start(instanceName);
        
        // Wait for event or timeout after 10 seconds
        bool received = await Task.WhenAny(eventReceived.Task, Task.Delay(10000))
            .ContinueWith(t => t.Result == eventReceived.Task);
        
        // Stop the instance
        KgsmClient.Instances.Stop(instanceName);
        
        // Assert
        received.Should().BeTrue("Should have received an instance status event");
    }
}

/// <summary>
/// Test fixture that creates a test instance for event tests and cleans it up afterward.
/// </summary>
public class EventTestInstanceFixture : IDisposable
{
    public string InstanceName { get; }
    public string InstallDir { get; }

    private readonly IKgsmClient _kgsmClient;
    
    public EventTestInstanceFixture()
    {
        // Use Factorio as the test instance for events as it's typically faster to start
        InstanceName = $"event-test-{Guid.NewGuid().ToString()[..8]}";
        InstallDir = Path.Combine("/tmp/kgsm-test", InstanceName);
        Directory.CreateDirectory(InstallDir);

        // Create service provider for KGSM client
        var services = new ServiceCollection();
        services.AddKgsmServices("/home/heisen/kgsm/kgsm.sh", "/home/heisen/kgsm/kgsm.sock");

        var serviceProvider = services.BuildServiceProvider();
        _kgsmClient = serviceProvider.GetRequiredService<IKgsmClient>();

        // Install Factorio instance for testing events
        var result = _kgsmClient.Instances.Install("factorio", InstallDir, name: InstanceName);
        if (!result.IsSuccess)
        {
            throw new Exception($"Failed to install test instance for events: {result.Stderr}");
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
