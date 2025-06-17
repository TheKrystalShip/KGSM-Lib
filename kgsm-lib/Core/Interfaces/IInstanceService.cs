using TheKrystalShip.KGSM.Core.Models;

namespace TheKrystalShip.KGSM.Core.Interfaces;

/// <summary>
/// Interface for managing instances in KGSM.
/// </summary>
public interface IInstanceService
{
    /// <summary>
    /// Gets a dictionary of all instances.
    /// </summary>
    /// <returns>A dictionary of instance names to instance objects.</returns>
    Dictionary<string, Instance> GetAll();

    /// <summary>
    /// Installs an instance of a blueprint.
    /// </summary>
    /// <param name="blueprintName">Name of the blueprint to install.</param>
    /// <param name="installDir">Optional installation directory.</param>
    /// <param name="version">Optional version to install.</param>
    /// <param name="name">Optional identifier used when creating the instance.</param>
    /// <returns>Result of the instance installation operation.</returns>
    KgsmResult Install(string blueprintName, string? installDir = null, string? version = null, string? name = null);

    /// <summary>
    /// Uninstalls an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to uninstall.</param>
    /// <returns>Result of the uninstallation operation.</returns>
    KgsmResult Uninstall(string instanceName);

    /// <summary>
    /// Gets the logs for an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to get logs for.</param>
    /// <returns>Result containing the instance logs.</returns>
    KgsmResult GetLogs(string instanceName);

    /// <summary>
    /// Gets the status of an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to get status for.</param>
    /// <returns>Result containing the instance status.</returns>
    KgsmResult GetStatus(string instanceName);

    /// <summary>
    /// Gets information about an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to get information for.</param>
    /// <returns>Result containing the instance information.</returns>
    KgsmResult GetInfo(string instanceName);

    /// <summary>
    /// Checks if an instance is currently active/running.
    /// </summary>
    /// <param name="instanceName">Instance name to check.</param>
    /// <returns>True if the instance is active, false otherwise.</returns>
    bool IsActive(string instanceName);

    /// <summary>
    /// Starts an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to start.</param>
    /// <returns>Result of the start operation.</returns>
    KgsmResult Start(string instanceName);

    /// <summary>
    /// Stops an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to stop.</param>
    /// <returns>Result of the stop operation.</returns>
    KgsmResult Stop(string instanceName);

    /// <summary>
    /// Restarts an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to restart.</param>
    /// <returns>Result of the restart operation.</returns>
    KgsmResult Restart(string instanceName);

    /// <summary>
    /// Gets the installed version of an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to get version for.</param>
    /// <returns>Result containing the installed version.</returns>
    KgsmResult GetInstalledVersion(string instanceName);

    /// <summary>
    /// Gets the latest available version for an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to get latest version for.</param>
    /// <returns>Result containing the latest version.</returns>
    KgsmResult GetLatestVersion(string instanceName);

    /// <summary>
    /// Checks if there's an update available for an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to check for updates.</param>
    /// <returns>Result indicating if an update is available.</returns>
    KgsmResult CheckUpdate(string instanceName);

    /// <summary>
    /// Updates an instance to the latest version.
    /// </summary>
    /// <param name="instanceName">Instance name to update.</param>
    /// <returns>Result of the update operation.</returns>
    KgsmResult Update(string instanceName);

    /// <summary>
    /// Gets a list of backups for an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to get backups for.</param>
    /// <returns>Result containing the list of backups.</returns>
    KgsmResult GetBackups(string instanceName);

    /// <summary>
    /// Creates a backup for an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to create backup for.</param>
    /// <returns>Result of the backup creation operation.</returns>
    KgsmResult CreateBackup(string instanceName);

    /// <summary>
    /// Restores a backup for an instance.
    /// </summary>
    /// <param name="instanceName">Instance name to restore backup for.</param>
    /// <param name="backupName">Name of the backup to restore.</param>
    /// <returns>Result of the backup restoration operation.</returns>
    KgsmResult RestoreBackup(string instanceName, string backupName);
}
