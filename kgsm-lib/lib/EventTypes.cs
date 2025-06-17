using System.Text.Json;
using System;

namespace TheKrystalShip.KGSM.Lib;

public class Blueprint
{
    public string Name { get; set; } = string.Empty;
    public string Ports { get; set; } = string.Empty;
    public string SteamAppId { get; set; } = string.Empty;
    public bool IsSteamAccountRequired { get; set; } = false;
    public string ExecutableFile { get; set; } = string.Empty;
    public string ExecutableSubdirectory { get; set; } = string.Empty;
    public string ExecutableArguments { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public string? StopCommand { get; set; } = null;
    public string? SaveCommand { get; set; } = null;

    public override string ToString()
    {
        return $"Blueprint: {Name}, " +
               $"Ports: {Ports}, " +
               $"SteamAppId: {SteamAppId}, " +
               $"IsSteamAccountRequired: {IsSteamAccountRequired}, " +
               $"ExecutableFile: {ExecutableFile}, " +
               $"ExecutableSubdirectory: {ExecutableSubdirectory}, " +
               $"ExecutableArguments: {ExecutableArguments}, " +
               $"LevelName: {LevelName}, " +
               $"StopCommand: {StopCommand ?? "None"}, " +
               $"SaveCommand: {SaveCommand ?? "None"}";
    }
}

public enum InstanceStatus
{
    Active,
    Inactive
}

public enum LifecycleManager
{
    Standalone,
    Systemd
}

public enum InstanceRuntime
{
    Native,
    Container
}

public record class Instance
{
    public string Name { get; set; } = string.Empty;
    public LifecycleManager LifecycleManager { get; set; } = LifecycleManager.Standalone;
    public InstanceStatus Status { get; set; } = InstanceStatus.Inactive;
    public string LogsDirectory { get; set; } = string.Empty;
    public string Directory { get; set; } = string.Empty;
    public DateTime InstallationDate { get; set; } = DateTime.MinValue;
    public string PID { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Blueprint { get; set; } = string.Empty;
    public string ServiceFile { get; set; } = string.Empty;
    public string SocketFile { get; set; } = string.Empty;
    public string FirewallRule { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Instance: {Name}, " +
               $"LifecycleManager: {LifecycleManager}, " +
               $"Status: {Status}, " +
               $"PID: {PID}, " +
               $"LogsDirectory: {LogsDirectory}, " +
               $"Directory: {Directory}, " +
               $"InstallationDate: {InstallationDate}, " +
               $"Version: {Version}, " +
               $"Blueprint: {Blueprint}, " +
               $"ServiceFile: {ServiceFile}, " +
               $"SocketFile: {SocketFile}, " +
               $"FirewallRule: {FirewallRule}";
    }
}

/// <summary>
/// Base class for all event data types.
/// This class contains common properties that all event data will inherit.
/// All events have an InstanceName property to identify the instance they are related to.
/// </summary>
public abstract class EventDataBase
{
    public string InstanceName { get; set; } = string.Empty;
}

public class EventWrapper
{
    public string EventType { get; set; } = string.Empty;
    // Using JsonElement to allow for dynamic deserialization of event data
    public JsonElement Data { get; set; }
}

public class InstanceCreatedData : EventDataBase
{
    public string Blueprint { get; set; } = string.Empty;
}

public class InstanceDirectoriesCreatedData : EventDataBase
{
}

public class InstanceFilesCreatedData : EventDataBase
{
}

public class InstanceDownloadStartedData : EventDataBase
{
}

public class InstanceDownloadFinishedData : EventDataBase
{
}

public class InstanceDownloadedData : EventDataBase
{
}

public class InstanceDeployStartedData : EventDataBase
{
}

public class InstanceDeployFinishedData : EventDataBase
{
}

public class InstanceDeployedData : EventDataBase
{
}

public class InstanceUpdateStartedData : EventDataBase
{
}

public class InstanceUpdateFinishedData : EventDataBase
{
}

public class InstanceUpdatedData : EventDataBase
{
}

public class InstanceVersionUpdatedData : EventDataBase
{
    public string OldVersion { get; set; } = string.Empty;
    public string NewVersion { get; set; } = string.Empty;
}

public class InstanceInstallationStartedData : EventDataBase
{
    public string Blueprint { get; set; } = string.Empty;
}

public class InstanceInstallationFinishedData : EventDataBase
{
    public string Blueprint { get; set; } = string.Empty;
}

public class InstanceInstalledData : EventDataBase
{
    public string Blueprint { get; set; } = string.Empty;
}

public class InstanceStartedData : EventDataBase
{
    public LifecycleManager LifecycleManager { get; set; }
}

public class InstanceStoppedData : EventDataBase
{
    public LifecycleManager LifecycleManager { get; set; }
}

public class InstanceBackupCreatedData : EventDataBase
{
    public string Source { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}

public class InstanceBackupRestoredData : EventDataBase
{
    public string Source { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}

public class InstanceFilesRemovedData : EventDataBase
{
}

public class InstanceDirectoriesRemovedData : EventDataBase
{
}

public class InstanceRemovedData : EventDataBase
{
}

public class InstanceUninstallStartedData : EventDataBase
{
}

public class InstanceUninstallFinishedData : EventDataBase
{
}

public class InstanceUninstalledData : EventDataBase
{
}