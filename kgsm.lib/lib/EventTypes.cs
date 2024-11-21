using System.Text.Json;

namespace TheKrystalShip.KGSM.Lib;

public enum LifecycleManager
{
    Standalone,
    Systemd
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