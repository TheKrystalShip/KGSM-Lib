using System.Text.Json;

namespace TheKrystalShip.KGSM.Lib;

/// <summary>
/// KgsmEvents is a class that listens for events from the KGSM Unix Socket.
/// It provides a way to register handlers for different event types
/// and automatically deserializes the event data based on the event type
/// received.
/// </summary>
public class KgsmEvents : IDisposable
{
    private readonly UnixSocketClient _client;
    private readonly CancellationTokenSource _cts;

    /// <summary>
    /// A dictionary to map event types to their corresponding data types.
    /// This mapping is used to deserialize the event data
    /// based on the event type received from the KGSM Unix Socket.
    /// </summary>
    private readonly Dictionary<string, Type> _eventTypeMapping = new()
    {
        { "instance_created", typeof(InstanceCreatedData) },

        { "instance_directories_created", typeof(InstanceDirectoriesCreatedData) },
        { "instance_files_created", typeof(InstanceFilesCreatedData) },
        
        { "instance_download_started", typeof(InstanceDownloadStartedData) },
        { "instance_download_finished", typeof(InstanceDownloadFinishedData) },
        { "instance_downloaded", typeof(InstanceDownloadedData) },
        
        { "instance_deploy_started", typeof(InstanceDeployStartedData) },
        { "instance_deploy_finished", typeof(InstanceDeployFinishedData) },
        { "instance_deployed", typeof(InstanceDeployedData) },
        
        { "instance_update_started", typeof(InstanceUpdateStartedData) },
        { "instance_update_finished", typeof(InstanceUpdateFinishedData) },
        { "instance_updated", typeof(InstanceUpdatedData) },
        
        { "instance_version_updated", typeof(InstanceVersionUpdatedData) },
        
        { "instance_installation_started", typeof(InstanceInstallationStartedData) },
        { "instance_installation_finished", typeof(InstanceInstallationFinishedData) },
        { "instance_installed", typeof(InstanceInstalledData) },
        
        { "instance_started", typeof(InstanceStartedData) },
        { "instance_stopped", typeof(InstanceStoppedData) },
        
        { "instance_backup_created", typeof(InstanceBackupCreatedData) },
        { "instance_backup_restored", typeof(InstanceBackupRestoredData) },
        
        { "instance_files_removed", typeof(InstanceFilesRemovedData) },
        { "instance_directories_removed", typeof(InstanceDirectoriesRemovedData) },
        
        { "instance_removed", typeof(InstanceRemovedData) },
        
        { "instance_uninstall_started", typeof(InstanceUninstallStartedData) },
        { "instance_uninstall_finished", typeof(InstanceUninstallFinishedData) },
        { "instance_uninstalled", typeof(InstanceUninstalledData) }
    };

    /// <summary>
    /// A dictionary to hold event handlers for the different event types.
    /// Each key is a Type representing the event data,
    /// and the value is a delegate that takes an instance of that type
    /// and returns a Task.
    /// </summary>
    private readonly Dictionary<Type, Delegate> _eventHandlers = new();

    /// <summary>
    /// Creates a new instance of the KgsmEvents class.
    /// It initializes the UnixSocketClient with the specified socket path.
    /// The socket path should point to the KGSM Unix Socket.
    /// Example usage:
    /// ```csharp
    /// var kgsmEvents = new KgsmEvents("/path/to/kgsm/socket");
    ///
    /// // Register event handlers here
    /// kgsmEvents.RegisterHandler<InstanceCreatedData>(async data =>
    /// {
    ///     // Handle the event data here
    /// });
    ///
    /// // Initialize the event listener
    /// kgsmEvents.Initialize();
    /// ```
    /// </summary>
    public KgsmEvents(string kgsmSocketPath)
    {
        if (string.IsNullOrEmpty(kgsmSocketPath))
            throw new ArgumentNullException(nameof(kgsmSocketPath));

        _client = new(kgsmSocketPath);
        _cts = new();
    }

    ~KgsmEvents()
    {
        this.Dispose();
    }

    /// <summary>
    /// Disposes of the KgsmEvents instance.
    /// This method cancels the event listening task and disposes of the
    /// UnixSocketClient.
    /// </summary>
    public void Dispose()
    {
        _cts.Cancel();
        _client.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Initializes the event listener and starts listening for events.
    /// This method should be called after registering all handlers.
    /// </summary>
    public void Initialize()
    {
        _client.EventReceived += OnEventReceivedAsync;

        // Start listening for events asynchronously.
        // This will run in the background until the KgsmEvents instance is disposed.
        if (_cts.IsCancellationRequested)
            throw new InvalidOperationException("Cannot initialize after disposal.");

        Task.Run(() => _client.StartListeningAsync(_cts.Token));
    }

    /// <summary>
    /// Registers an event handler for a specific event type.
    /// The handler should accept an instance of the event data type.
    /// Example usage:
    /// ```csharp
    /// kgsmEvents.RegisterHandler<InstanceCreatedData>(async data =>
    /// {
    ///     // Handle the event data here
    ///     Console.WriteLine($"Instance created: {data.InstanceId}");
    ///     return Task.CompletedTask;
    /// });
    /// ```
    /// </summary>
    public void RegisterHandler<T>(Func<T, Task> handler) where T : EventDataBase
    {
        _eventHandlers[typeof(T)] = async (EventDataBase data) => await handler((T)data);
    }

    /// <summary>
    /// Event handler for receiving events from the Unix socket.
    /// This method is called internally when an event is received.
    /// It deserializes the event data and invokes the appropriate handler.
    /// </summary>
    private async Task OnEventReceivedAsync(string message)
    {
        var eventWrapper = JsonSerializer.Deserialize<EventWrapper>(message);

        if (eventWrapper == null || string.IsNullOrWhiteSpace(eventWrapper.EventType))
        {
            return;
        }

        if (_eventTypeMapping.TryGetValue(eventWrapper.EventType, out var targetType))
        {
            var eventData = DeserializeEventData(targetType, eventWrapper.Data);
            await InvokeHandlerAsync(eventData);
        }
    }

    /// <summary>
    /// Deserializes the event data from JSON to the specified target type.
    /// This method is used internally to convert the JSON data
    /// </summary>
    private EventDataBase DeserializeEventData(Type targetType, JsonElement dataElement)
    {
        var json = dataElement.GetRawText();
        return JsonSerializer.Deserialize(json, targetType) as EventDataBase
               ?? throw new InvalidOperationException($"Failed to deserialize event data to {targetType}");
    }

    /// <summary>
    /// Invokes the registered handler for the given event data.
    /// This method is called internally when an event is received.
    /// </summary>
    private async Task InvokeHandlerAsync(EventDataBase eventData)
    {
        if (_eventHandlers.TryGetValue(eventData.GetType(), out var handler))
        {
            await ((Func<EventDataBase, Task>)handler).Invoke(eventData);
        }
    }
}