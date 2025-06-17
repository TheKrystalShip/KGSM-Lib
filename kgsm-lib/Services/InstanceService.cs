using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Core.Models;

namespace TheKrystalShip.KGSM.Services;

/// <summary>
/// Implementation of the IInstanceService interface for managing instances in KGSM.
/// </summary>
public class InstanceService : IInstanceService
{
    private readonly IProcessRunner _processRunner;
    private readonly string _kgsmPath;
    private readonly ILogger<InstanceService> _logger;

    /// <summary>
    /// Initializes a new instance of the InstanceService class.
    /// </summary>
    /// <param name="processRunner">The process runner to use for executing KGSM commands.</param>
    /// <param name="kgsmPath">The path to the KGSM executable.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public InstanceService(IProcessRunner processRunner, string kgsmPath, ILogger<InstanceService> logger)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        _kgsmPath = kgsmPath ?? throw new ArgumentNullException(nameof(kgsmPath));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Dictionary<string, Instance> GetAll()
    {
        _logger.LogDebug("Getting all instances");
        
        Dictionary<string, Instance> instances = new();
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instances", "--detailed", "--json");

        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to get instances: {Error}", result.Stderr);
            return instances;
        }

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        try
        {
            instances = JsonSerializer.Deserialize<Dictionary<string, Instance>>(
                result.Stdout,
                serializerOptions
            ) ?? new Dictionary<string, Instance>();
            
            _logger.LogDebug("Found {Count} instances", instances.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize instances");
        }

        return instances;
    }

    /// <inheritdoc/>
    public KgsmResult Install(string blueprintName, string? installDir = null, string? version = null, string? name = null)
    {
        ArgumentNullException.ThrowIfNull(blueprintName, nameof(blueprintName));
        
        _logger.LogDebug("Installing instance of blueprint {Blueprint}", blueprintName);
        
        List<string> args = new();
        
        args.Add("--create");
        args.Add(blueprintName);
        
        if (installDir is not null)
        {
            args.Add("--install-dir");
            args.Add(installDir);
        }

        if (version is not null)
        {
            args.Add("--version");
            args.Add(version);
        }

        if (name is not null)
        {
            args.Add("--name");
            args.Add(name);
        }

        ProcessResult result = _processRunner.Execute(_kgsmPath, args.ToArray());
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to install instance: {Error}", result.Stderr);
        }
        else
        {
            _logger.LogInformation("Successfully installed instance of blueprint {Blueprint}", blueprintName);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult Uninstall(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Uninstalling instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--uninstall", instanceName);
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to uninstall instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }
        else
        {
            _logger.LogInformation("Successfully uninstalled instance {InstanceName}", instanceName);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult GetLogs(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Getting logs for instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--logs");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to get logs for instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult GetStatus(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Getting status for instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--status");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to get status for instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult GetInfo(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Getting info for instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--info");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to get info for instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public bool IsActive(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Checking if instance {InstanceName} is active", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--is-active");

        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to check if instance {InstanceName} is active: {Error}", instanceName, result.Stderr);
            return false;
        }

        bool isActive = !result.Stdout.Contains("Inactive");
        _logger.LogDebug("Instance {InstanceName} is {Status}", instanceName, isActive ? "active" : "inactive");
        
        return isActive;
    }

    /// <inheritdoc/>
    public KgsmResult Start(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Starting instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--start");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to start instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }
        else
        {
            _logger.LogInformation("Successfully started instance {InstanceName}", instanceName);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult Stop(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Stopping instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--stop");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to stop instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }
        else
        {
            _logger.LogInformation("Successfully stopped instance {InstanceName}", instanceName);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult Restart(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Restarting instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--restart");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to restart instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }
        else
        {
            _logger.LogInformation("Successfully restarted instance {InstanceName}", instanceName);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult GetInstalledVersion(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Getting installed version for instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--version", "--installed");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to get installed version for instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult GetLatestVersion(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Getting latest version for instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--version", "--latest");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to get latest version for instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult CheckUpdate(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Checking for updates for instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--check-update");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to check for updates for instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult Update(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Updating instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--update");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to update instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }
        else
        {
            _logger.LogInformation("Successfully updated instance {InstanceName}", instanceName);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult GetBackups(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Getting backups for instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--backups");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to get backups for instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult CreateBackup(string instanceName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        
        _logger.LogDebug("Creating backup for instance {InstanceName}", instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--create-backup");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to create backup for instance {InstanceName}: {Error}", instanceName, result.Stderr);
        }
        else
        {
            _logger.LogInformation("Successfully created backup for instance {InstanceName}", instanceName);
        }

        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult RestoreBackup(string instanceName, string backupName)
    {
        ArgumentNullException.ThrowIfNull(instanceName, nameof(instanceName));
        ArgumentNullException.ThrowIfNull(backupName, nameof(backupName));
        
        _logger.LogDebug("Restoring backup {BackupName} for instance {InstanceName}", backupName, instanceName);
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--instance", instanceName, "--restore-backup", backupName);
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to restore backup {BackupName} for instance {InstanceName}: {Error}", backupName, instanceName, result.Stderr);
        }
        else
        {
            _logger.LogInformation("Successfully restored backup {BackupName} for instance {InstanceName}", backupName, instanceName);
        }

        return new KgsmResult(result);
    }
}
