using System.Net.Sockets;
using System.Text;

namespace TheKrystalShip.KGSM.Lib;

/// <summary>
/// UnixSocketClient is a class that listens for events from a Unix socket.
/// It provides a way to register an event handler that will be invoked
/// whenever a message is received from the socket.
/// </summary>
public class UnixSocketClient : IDisposable
{
    private bool _disposed = false;
    private readonly string _socketPath;
    public event Func<string, Task>? EventReceived;

    /// <summary>
    /// Initializes a new instance of the UnixSocketClient class with the specified socket path.
    /// Throws an ArgumentNullException if the socket path is null or empty.
    /// </summary>
    public UnixSocketClient(string socketPath)
    {
        ArgumentNullException.ThrowIfNull(socketPath, nameof(socketPath));

        _socketPath = socketPath;
    }

    ~UnixSocketClient()
    {
        Dispose(false);
    }

    /// <summary>
    /// <inheritdoc />
    /// This method deletes the Unix socket file if it exists.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Dispose managed resources here if needed
            if (File.Exists(_socketPath))
            {
                File.Delete(_socketPath);
            }
        }

        // Clean up unmanaged resources here if needed
        _disposed = true;
    }

    /// <summary>
    /// Starts listening for events on the Unix socket.
    /// This method will block until the listening is stopped or an exception occurs.
    /// It will invoke the EventReceived event handler whenever a message is received.
    /// </summary>
    public async Task StartListeningAsync(CancellationToken token)
    {
        if (File.Exists(_socketPath))
        {
            File.Delete(_socketPath);
        }

        using Socket server = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

        var endpoint = new UnixDomainSocketEndPoint(_socketPath);
        server.Bind(endpoint);
        server.Listen(backlog: 5);

        // Reader loop
        while (!token.IsCancellationRequested)
        {
            using Socket client = await server.AcceptAsync(token);
            using var networkStream = new NetworkStream(client);
        
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                
                if (string.IsNullOrEmpty(message))
                    continue;
                
                this.EventReceived?.Invoke(message);
            }
        }
    }
}