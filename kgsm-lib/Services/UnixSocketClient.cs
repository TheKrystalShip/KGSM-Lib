using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using TheKrystalShip.KGSM.Core.Interfaces;

namespace TheKrystalShip.KGSM.Services;

/// <summary>
/// Implementation of the IUnixSocketClient interface for Unix socket communication.
/// </summary>
public class UnixSocketClient : IUnixSocketClient, IDisposable
{
    private bool _disposed = false;
    private readonly string _socketPath;
    private readonly ILogger<UnixSocketClient> _logger;
    
    /// <inheritdoc/>
    public event Func<string, Task>? EventReceived;

    /// <summary>
    /// Initializes a new instance of the UnixSocketClient class with the specified socket path.
    /// </summary>
    /// <param name="socketPath">The path to the Unix socket.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public UnixSocketClient(string socketPath, ILogger<UnixSocketClient> logger)
    {
        ArgumentNullException.ThrowIfNull(socketPath, nameof(socketPath));
        
        _socketPath = socketPath;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Finalizer for the UnixSocketClient class.
    /// </summary>
    ~UnixSocketClient()
    {
        Dispose(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected implementation of Dispose pattern.
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Dispose managed resources here if needed
            if (File.Exists(_socketPath))
            {
                try
                {
                    File.Delete(_socketPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete socket file: {SocketPath}", _socketPath);
                }
            }
        }

        // Clean up unmanaged resources here if needed
        _disposed = true;
    }

    /// <inheritdoc/>
    public async Task StartListeningAsync(CancellationToken token)
    {
        if (File.Exists(_socketPath))
        {
            _logger.LogDebug("Socket file already exists, deleting: {SocketPath}", _socketPath);
            try
            {
                File.Delete(_socketPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete existing socket file: {SocketPath}", _socketPath);
                throw;
            }
        }

        _logger.LogInformation("Starting Unix socket listener on {SocketPath}", _socketPath);

        using Socket server = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

        try
        {
            var endpoint = new UnixDomainSocketEndPoint(_socketPath);
            server.Bind(endpoint);
            server.Listen(backlog: 5);
            
            _logger.LogInformation("Unix socket listener started successfully");

            // Reader loop
            while (!token.IsCancellationRequested)
            {
                _logger.LogDebug("Waiting for connection on Unix socket");
                
                Socket client;
                try
                {
                    client = await server.AcceptAsync(token);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Unix socket listener cancelled");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to accept connection on Unix socket");
                    continue;
                }

                _logger.LogDebug("Connection accepted on Unix socket");
                
                using (client)
                using (var networkStream = new NetworkStream(client))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    try
                    {
                        while ((bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                        {
                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                            
                            if (string.IsNullOrEmpty(message))
                                continue;
                            
                            _logger.LogDebug("Received message from Unix socket: {MessageLength} bytes", message.Length);
                            
                            try
                            {
                                if (EventReceived != null)
                                {
                                    await EventReceived.Invoke(message);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error processing message from Unix socket");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("Unix socket read operation cancelled");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error reading from Unix socket");
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Unix socket listener cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Unix socket listener");
            throw;
        }
        finally
        {
            _logger.LogInformation("Unix socket listener stopped");
        }
    }
}
