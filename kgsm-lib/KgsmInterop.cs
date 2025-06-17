using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging.Abstractions;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Core.Models;
using TheKrystalShip.KGSM.Events;
using TheKrystalShip.KGSM.Services;

namespace TheKrystalShip.KGSM;

/// <summary>
/// KgsmInterop is a class that provides an interface to interact with the KGSM (Krystal Game Server Manager).
/// It allows you to perform various operations such as creating blueprints, managing instances,
/// checking updates, and handling events through a Unix socket.
/// 
/// This class is kept for backward compatibility with the previous version of the library.
/// New code should use the IKgsmClient interface and its implementations.
/// </summary>
[Obsolete("This class is kept for backward compatibility. New code should use IKgsmClient interface.")]
public class KgsmInterop
{
    private readonly IKgsmClient _client;

    /// <summary>
    /// Gets the event service for handling KGSM events.
    /// </summary>
    public IEventService Events => _client.Events;

    /// <summary>
    /// Initializes a new instance of the KgsmInterop class with the specified KGSM path and socket path.
    /// Throws an ArgumentNullException if the kgsmPath is null or empty.
    /// </summary>
    public KgsmInterop(string kgsmPath, string kgsmSocketPath)
    {
        ArgumentNullException.ThrowIfNull(kgsmPath, nameof(kgsmPath));
        ArgumentNullException.ThrowIfNull(kgsmSocketPath, nameof(kgsmSocketPath));

        // Create the necessary services using the default nulllogger
        var processRunner = new ProcessRunner(NullLogger<ProcessRunner>.Instance);
        var socketClient = new UnixSocketClient(kgsmSocketPath, NullLogger<UnixSocketClient>.Instance);
        var eventService = new EventService(socketClient, NullLogger<EventService>.Instance);
        var blueprintService = new BlueprintService(processRunner, kgsmPath, NullLogger<BlueprintService>.Instance);
        var instanceService = new InstanceService(processRunner, kgsmPath, NullLogger<InstanceService>.Instance);

        _client = new KgsmClient(
            kgsmPath,
            processRunner,
            blueprintService,
            instanceService,
            eventService,
            NullLogger<KgsmClient>.Instance);
    }    // General
    /// <summary>
    /// Prints the help message
    /// </summary>
    public KgsmResult Help() => _client.Help();

    /// <summary>
    /// Prints the help message for the interactive mode
    /// </summary>
    public KgsmResult HelpInteractive() => _client.HelpInteractive();

    /// <summary>
    /// Update KGSM if a new version is available
    /// </summary>
    public KgsmResult Update() => _client.UpdateKgsm();

    /// <summary>
    /// Prints the server's public IP address
    /// </summary>
    public KgsmResult GetIp() => _client.GetIp();

    /// <summary>
    /// Print the version information for KGSM
    /// </summary>
    public KgsmResult GetVersion() => _client.GetVersion();    // Blueprints

    /// <summary>
    /// Prints a list of all available blueprints
    /// </summary>
    public Dictionary<string, Blueprint> GetBlueprints()
    {
        return _client.Blueprints.GetAll();
    }    /// <summary>
    /// Create an instance of a blueprint
    /// </summary>
    /// <param name="blueprintName">Name of the blueprint to install</param>
    /// <param name="installDir">Optional installation directory</param>
    /// <param name="version">Optional version to install</param>
    /// <param name="name">Optional identifier used when creating the instance</param>
    public KgsmResult Install(string blueprintName, string? installDir = null, string? version = null, string? name = null) 
    {
        return _client.Instances.Install(blueprintName, installDir, version, name);
    }    // Instances

    /// <summary>
    /// Uninstall an instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Uninstall(string instance) 
        => _client.Instances.Uninstall(instance);

    /// <summary>
    /// Prints a list of all instances
    /// </summary>
    public Dictionary<string, Instance> GetInstances()
    {
        return _client.Instances.GetAll();
    }

    /// <summary>
    /// Print the last 10 lines for the instance log
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult GetLogs(string instance)
        => _client.Instances.GetLogs(instance);

    /// <summary>
    /// Print a detailed message about the current status of the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Status(string instance)
        => _client.Instances.GetStatus(instance);

    /// <summary>
    /// Print a detailed message with information about the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Info(string instance)
        => _client.Instances.GetInfo(instance);

    /// <summary>
    /// Print if the instance is currently active/running
    /// </summary>
    /// <param name="instance">Instance name</param>
    public bool IsActive(string instance)
        => _client.Instances.IsActive(instance);

    /// <summary>
    /// Start the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Start(string instance)
        => _client.Instances.Start(instance);    /// <summary>
    /// Stop the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Stop(string instance)
        => _client.Instances.Stop(instance);

    /// <summary>
    /// Restart the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Restart(string instance)
        => _client.Instances.Restart(instance);

    /// <summary>
    /// Print the installed version of the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult GetInstalledVersion(string instance)
        => _client.Instances.GetInstalledVersion(instance);

    /// <summary>
    /// Print the latest available version of the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult GetLatestVersion(string instance)
        => _client.Instances.GetLatestVersion(instance);

    /// <summary>
    /// Check if there's an update available for the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult CheckUpdate(string instance)
        => _client.Instances.CheckUpdate(instance);

    /// <summary>
    /// Run the update process for the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Update(string instance)
        => _client.Instances.Update(instance);

    /// <summary>
    /// Print a list of the created instance backups
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult GetBackups(string instance)
        => _client.Instances.GetBackups(instance);

    /// <summary>
    /// Create a new backup for the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult CreateBackup(string instance)
        => _client.Instances.CreateBackup(instance);

    /// <summary>
    /// Restore a specific backup for the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    /// <param name="backupName">
    /// Name of the backup to restore.
    /// Call GetBackups in order to get a list of available options
    /// </param>
    public KgsmResult RestoreBackup(string instance, string backupName)
        => _client.Instances.RestoreBackup(instance, backupName);

    /// <summary>
    /// Execute Ad-Hoc commands
    /// Useful if the command you're trying to execute hasn't been mapped
    /// by KgsmInterop.
    /// </summary>
    /// <param name="args">Arguments to send to KGSM</param>
    /// <returns>KgsmResult</returns>
    public KgsmResult AdHoc(params string[] args)
        => _client.AdHoc(args);
}