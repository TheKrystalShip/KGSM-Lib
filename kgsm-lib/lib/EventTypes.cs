using System.Text.Json;

namespace TheKrystalShip.KGSM.Lib;

public class Blueprint
{
    public string Name { get; set; } = "";
    public string Port { get; set; } = "";
    public string AppId { get; set; } = "";
    public bool SteamAccountRequired { get; set; } = false;
    public string LaunchBin { get; set; } = "";
    public string LevelName { get; set; } = "";
    public string InstallSubdirectory { get; set; } = "";
    public string LaunchArgs { get; set; } = "";
    public string? StopCommand { get; set; } = null;
    public string? SaveCommand { get; set; } = null;

    public override string ToString()
    {
        return $"Blueprint: {Name}, " +
               $"Port: {Port}, " +
               $"AppId: {AppId}, " +
               $"SteamAccountRequired: {SteamAccountRequired}, " +
               $"LaunchBin: {LaunchBin}, " +
               $"LevelName: {LevelName}, " +
               $"InstallSubdirectory: {InstallSubdirectory}, " +
               $"LaunchArgs: {LaunchArgs}, " +
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

public class Instance
{
    public string Name { get; set; } = "";
    public LifecycleManager LifecycleManager { get; set ;} = LifecycleManager.Standalone;
    public InstanceStatus Status { get; set; } = InstanceStatus.Inactive;
    public string? PID { get; set; } = "";
    public string? LogsDirectory { get; set; } = "";
    public string Directory { get; set; } = "";
    public DateTime InstallationDate { get; set; }
    public string Version { get; set; } = "";
    public string Blueprint { get; set; } = "";
    public string? ServiceFile { get; set; } = null;
    public string? SocketFile { get; set; } = null;
    public string? FirewallRule { get; set; } = null;

    public override string ToString()
    {
        return $"Instance: {Name}, " +
               $"LifecycleManager: {LifecycleManager}, " +
               $"Status: {Status}, " +
               $"PID: {PID ?? "None"}, " +
               $"LogsDirectory: {LogsDirectory ?? "None"}, " +
               $"Directory: {Directory}, " +
               $"InstallationDate: {InstallationDate}, " +
               $"Version: {Version}, " +
               $"Blueprint: {Blueprint}, " +
               $"ServiceFile: {ServiceFile ?? "None"}, " +
               $"SocketFile: {SocketFile ?? "None"}, " +
               $"FirewallRule: {FirewallRule ?? "None"}";
    }
}

public abstract class EventDataBase { }

public class EventWrapper
{
    public string EventType { get; set; } = "";
    public JsonElement Data { get; set; } // Using JsonElement for dynamic deserialization
}

public class InstanceCreatedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
    public string Blueprint { get; set; } = "";
}

public class InstanceDirectoriesCreatedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceFilesCreatedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceDownloadStartedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceDownloadFinishedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceDownloadedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceDeployStartedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceDeployFinishedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceDeployedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceUpdateStartedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceUpdateFinishedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceUpdatedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceVersionUpdatedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
    public string OldVersion { get; set; } = "";
    public string NewVersion { get; set; } = "";
}

public class InstanceInstallationStartedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
    public string Blueprint { get; set; } = "";
}

public class InstanceInstallationFinishedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
    public string Blueprint { get; set; } = "";
}

public class InstanceInstalledData : EventDataBase
{
    public string InstanceId { get; set; } = "";
    public string Blueprint { get; set; } = "";
}

public class InstanceStartedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
    public LifecycleManager LifecycleManager { get; set; }
}

public class InstanceStoppedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
    public LifecycleManager LifecycleManager { get; set; }
}

public class InstanceBackupCreatedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
    public string Source { get; set; } = "";
    public string Version { get; set; } = "";
}

public class InstanceBackupRestoredData : EventDataBase
{
    public string InstanceId { get; set; } = "";
    public string Source { get; set; } = "";
    public string Version { get; set; } = "";
}

public class InstanceFilesRemovedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceDirectoriesRemovedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceRemovedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceUninstallStartedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceUninstallFinishedData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}

public class InstanceUninstalledData : EventDataBase
{
    public string InstanceId { get; set; } = "";
}