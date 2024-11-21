using System.Text.Json;

namespace TheKrystalShip.KGSM.Lib;

public class KgsmEvents : IDisposable
{
    private readonly UnixSocketClient _client;
    private readonly CancellationTokenSource _cts;

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

    private readonly Dictionary<Type, Delegate> _eventHandlers = new();

    public KgsmEvents(string kgsmSocketPath)
    {
        if (string.IsNullOrEmpty(kgsmSocketPath))
            throw new ArgumentNullException(nameof(kgsmSocketPath));

        _client = new(kgsmSocketPath);
        _cts = new();
    }

    ~KgsmEvents()
    {
        Dispose();
    }

    public void Dispose()
    {
        _cts.Cancel();
    }

    public void Initialize()
    {
        _client.EventReceived += OnEventReceivedAsync;
        Task.Run(() => _client.StartListeningAsync(_cts.Token));
    }

    public void RegisterHandler<T>(Func<T, Task> handler) where T : EventDataBase
    {
        _eventHandlers[typeof(T)] = handler;
    }

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

    private EventDataBase DeserializeEventData(Type targetType, JsonElement dataElement)
    {
        var json = dataElement.GetRawText();
        return JsonSerializer.Deserialize(json, targetType) as EventDataBase
               ?? throw new InvalidOperationException($"Failed to deserialize event data to {targetType}");
    }

    private async Task InvokeHandlerAsync(EventDataBase eventData)
    {
        if (_eventHandlers.TryGetValue(eventData.GetType(), out var handler) && handler is Func<EventDataBase, Task> eventHandler)
        {
            await eventHandler.Invoke(eventData);
        }
    }
}