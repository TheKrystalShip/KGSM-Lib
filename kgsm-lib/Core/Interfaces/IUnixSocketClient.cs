namespace TheKrystalShip.KGSM.Core.Interfaces;

/// <summary>
/// Interface for a Unix socket client that listens for events.
/// </summary>
public interface IUnixSocketClient : IDisposable
{
    /// <summary>
    /// Event that is triggered when a message is received from the Unix socket.
    /// </summary>
    event Func<string, Task>? EventReceived;

    /// <summary>
    /// Starts listening for events on the Unix socket.
    /// </summary>
    /// <param name="token">Cancellation token to stop listening.</param>
    /// <returns>A task that completes when listening is stopped.</returns>
    Task StartListeningAsync(CancellationToken token);
}
