using _pi = TheKrystalShip.KGSM.Lib.ProcessIntrop;

using TheKrystalShip.KGSM.Lib;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheKrystalShip.KGSM;

public class KgsmInterop
{
    private string _kgsmPath;
    public KgsmEvents Events { get; private set; }

    public KgsmInterop(string kgsmPath, string kgsmSocketPath)
    {
        if (string.IsNullOrEmpty(kgsmPath))
            throw new ArgumentNullException(nameof(kgsmPath));

        _kgsmPath = kgsmPath;
        Events = new(kgsmSocketPath);
        Events.Initialize();
    }

    // General
    /// <summary>
    /// Prints the help message
    /// </summary>
    public KgsmResult Help()
        => _pi.Execute(ref _kgsmPath, "--help");

    /// <summary>
    /// Prints the help message for the interactive mode
    /// </summary>
    public KgsmResult HelpInteractive()
        => _pi.Execute(ref _kgsmPath, "--help", "--interactive");

    /// <summary>
    /// Update KGSM if a new version is available
    /// </summary>
    public KgsmResult Update()
        => _pi.Execute(ref _kgsmPath, "--update");

    /// <summary>
    /// Prints the server's public IP address
    /// </summary>
    public KgsmResult GetIp()
        => _pi.Execute(ref _kgsmPath, "--ip");

    /// <summary>
    /// Print the version information for KGSM
    /// </summary>
    public KgsmResult GetVersion()
        => _pi.Execute(ref _kgsmPath, "--version");

    // Blueprints

    /// <summary>
    /// Create a new blueprint
    /// </summary>
    /// <param name="blueprint">Blueprint object populated</param>
    public KgsmResult CreateBlueprint(Blueprint blueprint)
        => _pi.Execute(ref _kgsmPath,
                "--create-blueprint",
                "--name", blueprint.Name,
                "--port", blueprint.Port,
                "--launch-bin", blueprint.LaunchBin,
                "--level-name", blueprint.LevelName,
                "--app-id", blueprint.AppId.ToString(),
                "--steam-auth-level", blueprint.SteamAccountRequired ? "1" : "0",
                "--install-subdirectory", blueprint.InstallSubdirectory,
                "--launch-args", blueprint.LaunchArgs,
                "--stop-command", blueprint.StopCommand ?? string.Empty,
                "--save-command", blueprint.SaveCommand ?? string.Empty
            );


    /// <summary>
    /// Print the help message for creating a new blueprint
    /// </summary>
    public KgsmResult CreateBlueprintHelp()
        => _pi.Execute(ref _kgsmPath, "--create-blueprint", "--help");

    /// <summary>
    /// Prints a list of all available blueprints
    /// </summary>
    public Dictionary<string, Blueprint> GetBlueprints()
    {
        Dictionary<string, Blueprint> blueprints = new();
        KgsmResult result = _pi.Execute(ref _kgsmPath, "--blueprints", "--detailed", "--json");

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true  
        };
        serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        serializerOptions.Converters.Add(new JsonStringToBoolConverter());

        blueprints = JsonSerializer.Deserialize<Dictionary<string, Blueprint>>(
            result.Stdout,
            serializerOptions
        ) ?? throw new InvalidOperationException("Failed to deserialize response");

        return blueprints;
    }

    /// <summary>
    /// Create an instance of a blueprint
    /// </summary>
    /// <param name="blueprintName">Name of the blueprint to install</param>
    /// <param name="installDir">Optional installation directory</param>
    /// <param name="version">Optional version to install</param>
    /// <param name="id">Optional identifier used when creating the instance</param>
    public KgsmResult Install(string blueprintName, string? installDir = null, string? version = null, string? id = null) 
    {
        List<string> args = [];

        args.Add("--install");
        args.Add(blueprintName);
        
        if (installDir is not null) {
            args.Add("--install-dir");
            args.Add(installDir);
        }

        if (version is not null) {
            args.Add("--version");
            args.Add(version);
        }

        if (id is not null) {
            args.Add("--id");
            args.Add(id);
        }

        return _pi.Execute(ref _kgsmPath, [.. args]);
    }

    // Instances

    /// <summary>
    /// Uninstall an instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Uninstall(string instance)
        => _pi.Execute(ref _kgsmPath, "--uninstall", instance);

    /// <summary>
    /// Prints a list of all instances
    /// </summary>
    public Dictionary<string, Instance> GetInstances()
    {
        Dictionary<string, Instance> instances = new();
        KgsmResult result = _pi.Execute(ref _kgsmPath, "--instances", "--detailed", "--json");

        var serializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        instances = JsonSerializer.Deserialize<Dictionary<string, Instance>>(
            result.Stdout,
            serializerOptions
        ) ?? throw new InvalidOperationException("Failed to deserialize result");

        return instances;
    }

    /// <summary>
    /// Print the last 10 lines for the instance log
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult GetLogs(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--logs");

    /// <summary>
    /// Print a detailed message about the current status of the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Status(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--status");

    /// <summary>
    /// Print a detailed message with information about the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Info(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--info");

    /// <summary>
    /// Print if the instance is currently active/running
    /// </summary>
    /// <param name="instance">Instance name</param>
    public bool IsActive(string instance)
    {
        ProcessResult result = _pi.Execute(ref _kgsmPath, "--instance", instance, "--is-active");

        if (result.ExitCode != 0)
            return false;

        if (result.Stdout.Contains("Inactive"))
            return false;

        return true;
    }

    /// <summary>
    /// Start the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Start(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--start");

    /// <summary>
    /// Stop the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Stop(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--stop");

    /// <summary>
    /// Restart the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Restart(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--restart");

    /// <summary>
    /// Print the installed version of the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult GetInstalledVersion(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--version", "--installed");

    /// <summary>
    /// Print the latest available version of the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult GetLatestVersion(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--version", "--latest");

    /// <summary>
    /// Check if there's an update available for the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult CheckUpdate(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--check-update");

    /// <summary>
    /// Run the update process for the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult Update(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--update");

    /// <summary>
    /// Print a list of the created instance backups
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult GetBackups(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--backups");

    /// <summary>
    /// Create a new backup for the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    public KgsmResult CreateBackup(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--create-backup");

    /// <summary>
    /// Restore a specific backup for the instance
    /// </summary>
    /// <param name="instance">Instance name</param>
    /// <param name="backupName">
    /// Name of the backup to restore.
    /// Call GetBackups in order to get a list of available options
    /// </param>
    public KgsmResult RestoreBackup(string instance, string backupName)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--restore-backup", backupName);

    /// <summary>
    /// Execute Ad-Hoc commands
    /// Useful if the command you're trying to execute hasn't been mapped
    /// by KgsmInterop.
    /// </summary>
    /// <param name="args">Arguments to send to KGSM</param>
    /// <returns>KgsmResult</returns>
    public KgsmResult AdHoc(params string[] args)
        => _pi.Execute(ref _kgsmPath, args);
}