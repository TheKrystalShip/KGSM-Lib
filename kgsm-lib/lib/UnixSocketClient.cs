using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheKrystalShip.KGSM.Lib;

public class UnixSocketClient : IDisposable
{
    private readonly string _socketPath;
    public event Func<string, Task>? EventReceived;

    public UnixSocketClient(string socketPath)
    {
        if (string.IsNullOrEmpty(socketPath))
            throw new ArgumentNullException(nameof(socketPath));

        _socketPath = socketPath;
    }

    ~UnixSocketClient()
    {
        this.Dispose();
    }

    public void Dispose()
    {
        if (File.Exists(_socketPath))
        {
            File.Delete(_socketPath);
        }
    }

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