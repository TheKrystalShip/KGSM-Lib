using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Integration;

/// <summary>
/// Integration tests for the <see cref="IUnixSocketClient"/> with real KGSM socket.
/// </summary>
public class UnixSocketIntegrationTests : OutputTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnixSocketIntegrationTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    public UnixSocketIntegrationTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task StartListening_ShouldNotThrow()
    {
        // Arrange
        var socketClient = ServiceProvider.GetRequiredService<IUnixSocketClient>();
        
        // Act & Assert - Should not throw
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        Func<Task> startListening = async () => 
        {
            await socketClient.StartListeningAsync(cts.Token);
        };
        await startListening.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EventReceived_ShouldTriggerWhenEventsOccur()
    {
        // Arrange
        var socketClient = ServiceProvider.GetRequiredService<IUnixSocketClient>();
        var eventReceived = new TaskCompletionSource<string>();
        
        socketClient.EventReceived += (message) => 
        {
            eventReceived.TrySetResult(message);
            return Task.CompletedTask;
        };
        
        // Start listening in background
        var cts = new CancellationTokenSource();
        var listeningTask = Task.Run(async () => 
        {
            await socketClient.StartListeningAsync(cts.Token);
        });
        
        try
        {
            // Generate an event by starting and stopping an instance
            string instanceName = $"test-event-{Guid.NewGuid().ToString()[..8]}";
            
            // Install a test instance to generate events
            var kgsmClient = ServiceProvider.GetRequiredService<IKgsmClient>();
            var installResult = kgsmClient.Instances.Install("factorio", Path.Combine("/tmp/kgsm-test", instanceName), name: instanceName);
            installResult.IsSuccess.Should().BeTrue();
            
            // Start the instance to generate events
            kgsmClient.Instances.Start(instanceName);
            
            // Wait for event or timeout after 10 seconds
            var timeoutTask = Task.Delay(10000);
            var completedTask = await Task.WhenAny(eventReceived.Task, timeoutTask);
            
            // Cleanup
            kgsmClient.Instances.Stop(instanceName);
            kgsmClient.Instances.Uninstall(instanceName);
            
            // Assert
            completedTask.Should().Be(eventReceived.Task, "Should have received an event");
            if (completedTask == eventReceived.Task)
            {
                var message = await eventReceived.Task;
                message.Should().NotBeNullOrEmpty("Event message should not be empty");
                Output.WriteLine($"Event received: {message}");
            }
        }
        finally
        {
            // Cancel listening
            cts.Cancel();
            try { await listeningTask; } catch { /* Ignore exceptions from cancellation */ }
            socketClient.Dispose();
        }
    }
}
