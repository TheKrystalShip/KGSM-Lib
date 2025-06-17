namespace TheKrystalShip.KGSM.Core.Models;

/// <summary>
/// Represents the status of an instance.
/// </summary>
public enum InstanceStatus
{
    /// <summary>
    /// The instance is active/running.
    /// </summary>
    Active,
    
    /// <summary>
    /// The instance is inactive/stopped.
    /// </summary>
    Inactive
}

/// <summary>
/// Represents the lifecycle manager for an instance.
/// </summary>
public enum LifecycleManager
{
    /// <summary>
    /// The instance is managed standalone.
    /// </summary>
    Standalone,
    
    /// <summary>
    /// The instance is managed by systemd.
    /// </summary>
    Systemd
}

/// <summary>
/// Represents the runtime environment for an instance.
/// </summary>
public enum InstanceRuntime
{
    /// <summary>
    /// The instance runs natively.
    /// </summary>
    Native,
    
    /// <summary>
    /// The instance runs in a container.
    /// </summary>
    Container
}

/// <summary>
/// Represents an instance of a game server.
/// </summary>
public record class Instance
{
    /// <summary>
    /// Gets or sets the name of the instance.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the lifecycle manager for the instance.
    /// </summary>
    public LifecycleManager LifecycleManager { get; set; } = LifecycleManager.Standalone;
    
    /// <summary>
    /// Gets or sets the status of the instance.
    /// </summary>
    public InstanceStatus Status { get; set; } = InstanceStatus.Inactive;
    
    /// <summary>
    /// Gets or sets the logs directory for the instance.
    /// </summary>
    public string LogsDirectory { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the directory for the instance.
    /// </summary>
    public string Directory { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the installation date of the instance.
    /// </summary>
    public DateTime InstallationDate { get; set; } = DateTime.MinValue;
    
    /// <summary>
    /// Gets or sets the process ID of the instance.
    /// </summary>
    public string PID { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the version of the instance.
    /// </summary>
    public string Version { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the blueprint name for the instance.
    /// </summary>
    public string Blueprint { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the service file for the instance.
    /// </summary>
    public string ServiceFile { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the socket file for the instance.
    /// </summary>
    public string SocketFile { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the firewall rule for the instance.
    /// </summary>
    public string FirewallRule { get; set; } = string.Empty;

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
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
