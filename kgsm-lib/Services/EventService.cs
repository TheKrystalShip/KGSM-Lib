using System.Text.Json;
using Microsoft.Extensions.Logging;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Events;

namespace TheKrystalShip.KGSM.Services;

/// <summary>
/// Implementation of the IEventService interface for handling KGSM events.
/// </summary>
public class EventService : IEventService
{
    private readonly IUnixSocketClient _client;
    private readonly CancellationTokenSource _cts;
    private readonly ILogger<EventService> _logger;

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
    /// </summary>
    private readonly Dictionary<Type, Delegate> _eventHandlers = new();

    /// <summary>
    /// Initializes a new instance of the EventService class.
    /// </summary>
    /// <param name="client">The Unix socket client to use for communication.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public EventService(IUnixSocketClient client, ILogger<EventService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cts = new CancellationTokenSource();
    }

    /// <summary>
    /// Finalizer for the EventService class.
    /// </summary>
    ~EventService()
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
        if (disposing)
        {
            _cts.Cancel();
            _cts.Dispose();
            _client.Dispose();
        }
    }

    /// <inheritdoc/>
    public void Initialize()
    {
        _logger.LogInformation("Initializing event service");
        
        _client.EventReceived += OnEventReceivedAsync;

        if (_cts.IsCancellationRequested)
            throw new InvalidOperationException("Cannot initialize after disposal.");

        _logger.LogDebug("Starting event listener");
        
        // Start listening for events asynchronously.
        Task.Run(() => _client.StartListeningAsync(_cts.Token));
        
        _logger.LogInformation("Event service initialized");
    }

    /// <inheritdoc/>
    public void RegisterHandler<T>(Func<T, Task> handler) where T : EventDataBase
    {
        ArgumentNullException.ThrowIfNull(handler, nameof(handler));
        
        Type eventType = typeof(T);
        _logger.LogDebug("Registering handler for event type {EventType}", eventType.Name);
        
        _eventHandlers[eventType] = async (EventDataBase data) => await handler((T)data);
    }

    /// <summary>
    /// Event handler for receiving events from the Unix socket.
    /// </summary>
    /// <param name="message">The event message received from the Unix socket.</param>
    private async Task OnEventReceivedAsync(string message)
    {
        _logger.LogDebug("Received event message: {MessageLength} bytes", message.Length);
        
        try
        {
            var eventWrapper = JsonSerializer.Deserialize<EventWrapper>(message);

            if (eventWrapper == null || string.IsNullOrWhiteSpace(eventWrapper.EventType))
            {
                _logger.LogWarning("Invalid event wrapper received");
                return;
            }

            _logger.LogDebug("Processing event of type {EventType}", eventWrapper.EventType);
            
            if (_eventTypeMapping.TryGetValue(eventWrapper.EventType, out var targetType))
            {
                _logger.LogDebug("Deserializing event data to type {TargetType}", targetType.Name);
                
                var eventData = DeserializeEventData(targetType, eventWrapper.Data);
                await InvokeHandlerAsync(eventData);
            }
            else
            {
                _logger.LogWarning("Unknown event type received: {EventType}", eventWrapper.EventType);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize event message");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event message");
        }
    }

    /// <summary>
    /// Deserializes the event data from JSON to the specified target type.
    /// </summary>
    /// <param name="targetType">The type to deserialize to.</param>
    /// <param name="dataElement">The JSON data to deserialize.</param>
    /// <returns>The deserialized event data.</returns>
    private EventDataBase DeserializeEventData(Type targetType, JsonElement dataElement)
    {
        var json = dataElement.GetRawText();
        
        _logger.LogTrace("Deserializing JSON: {Json}", json);
        
        var result = JsonSerializer.Deserialize(json, targetType) as EventDataBase
                     ?? throw new InvalidOperationException($"Failed to deserialize event data to {targetType}");

        _logger.LogDebug("Successfully deserialized event data to {TargetType}", targetType.Name);
        
        return result;
    }

    /// <summary>
    /// Invokes the registered handler for the given event data.
    /// </summary>
    /// <param name="eventData">The event data to handle.</param>
    private async Task InvokeHandlerAsync(EventDataBase eventData)
    {
        Type eventType = eventData.GetType();
        
        if (_eventHandlers.TryGetValue(eventType, out var handler))
        {
            _logger.LogDebug("Invoking handler for event type {EventType}", eventType.Name);
            
            try
            {
                await ((Func<EventDataBase, Task>)handler).Invoke(eventData);
                
                _logger.LogDebug("Handler for {EventType} completed successfully", eventType.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in event handler for {EventType}", eventType.Name);
            }
        }
        else
        {
            _logger.LogDebug("No handler registered for event type {EventType}", eventType.Name);
        }
    }
}
