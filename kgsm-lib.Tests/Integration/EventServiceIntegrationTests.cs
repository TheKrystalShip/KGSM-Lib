using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Events;
using TheKrystalShip.KGSM.Extensions;
using TheKrystalShip.KGSM.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Integration;

/// <summary>
/// Integration tests for <see cref="IEventService"/> that listen for real KGSM events.
/// </summary>
[Collection("GameServerTests")]
public class EventServiceIntegrationTests : OutputTestBase
{
    private readonly FactorioTestFixture _factorioFixture;
    private readonly ILogger<EventServiceIntegrationTests> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventServiceIntegrationTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    /// <param name="factorioFixture">The Factorio test fixture.</param>
    public EventServiceIntegrationTests(
        ITestOutputHelper output, 
        FactorioTestFixture factorioFixture) : base(output)
    {
        _factorioFixture = factorioFixture;
        _logger = LoggerFactory.CreateLogger<EventServiceIntegrationTests>();
    }

    [Fact]
    public async Task EventService_ShouldDetectInstanceStartupAndShutdown()
    {
        // Arrange
        string instanceName = _factorioFixture.InstanceName;
        var startEventReceived = new TaskCompletionSource<bool>();
        var stopEventReceived = new TaskCompletionSource<bool>();
        
        _logger.LogInformation("Setting up event listener for instance: {InstanceName}", instanceName);

        // Ensure instance is stopped initially
        if (KgsmClient.Instances.IsActive(instanceName))
        {
            _logger.LogInformation("Stopping instance before test");
            KgsmClient.Instances.Stop(instanceName);
            await Task.Delay(2000); // Give it time to stop
        }

        // Act - Initialize and register event handlers
        KgsmClient.Events.Initialize();
        
        // Register handlers for start and stop events
        KgsmClient.Events.RegisterHandler<InstanceStartedData>(evt =>
        {
            _logger.LogInformation("Received start event for instance: {InstanceName}", 
                evt.InstanceName);
            
            if (evt.InstanceName == instanceName)
            {
                startEventReceived.TrySetResult(true);
            }
            
            return Task.CompletedTask;
        });

        // Register handler for stopped events
        KgsmClient.Events.RegisterHandler<InstanceStoppedData>(evt =>
        {
            _logger.LogInformation("Received stop event for instance: {InstanceName}", 
                evt.InstanceName);
            
            if (evt.InstanceName == instanceName)
            {
                stopEventReceived.TrySetResult(true);
            }
            
            return Task.CompletedTask;
        });

        // Start the instance
        _logger.LogInformation("Starting instance {InstanceName}", instanceName);
        KgsmClient.Instances.Start(instanceName);
        
        // Wait for start event or timeout after 15 seconds
        var startEventTask = await Task.WhenAny(
            startEventReceived.Task, 
            Task.Delay(15000).ContinueWith(_ => false));
        bool startReceived = startEventTask == startEventReceived.Task && await startEventTask;
        
        // Stop the instance
        _logger.LogInformation("Stopping instance {InstanceName}", instanceName);
        KgsmClient.Instances.Stop(instanceName);
        
        // Wait for stop event or timeout after 15 seconds
        var stopEventTask = await Task.WhenAny(
            stopEventReceived.Task, 
            Task.Delay(15000).ContinueWith(_ => false));
        bool stopReceived = stopEventTask == stopEventReceived.Task && await stopEventTask;

        // Assert
        _logger.LogInformation("Start event received: {StartReceived}, Stop event received: {StopReceived}", 
            startReceived, stopReceived);
            
        startReceived.Should().BeTrue("Should have received an instance started event");
        stopReceived.Should().BeTrue("Should have received an instance stopped event");
    }
}
