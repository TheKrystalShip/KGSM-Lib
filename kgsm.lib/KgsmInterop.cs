using _pi = TheKrystalShip.KGSM.Lib.ProcessIntrop;

namespace TheKrystalShip.KGSM;

public class KgsmInterop(string kgsmPath)
{
    private string _kgsmPath = kgsmPath;

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
    /// Update KGSM to the latest version, skipping the version check
    /// </summary>
    public KgsmResult UpdateForce()
        => _pi.Execute(ref _kgsmPath, "--update", "--force");

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
    public KgsmResult CreateBlueprint(KgsmBlueprint blueprint)
        => _pi.Execute(ref _kgsmPath, "--create-blueprint",
                "--name", blueprint.Name,
                "--port", blueprint.Port,
                "--launch-bin", blueprint.LaunchBin,
                "--level-name", blueprint.LevelName,
                "--app-id", blueprint.SteamAppId.ToString(),
                "--steam-auth-level", blueprint.SteamAccountRequired ? "1" : "0",
                "--install-subdirectory", blueprint.LaunchBinSubdirectory,
                "--launch-args", blueprint.LaunchBinArgs,
                "--stop-command", blueprint.StopCommand,
                "--save-command", blueprint.SaveCommand
            );


    /// <summary>
    /// Print the help message for creating a new blueprint
    /// </summary>
    public KgsmResult CreateBlueprintHelp()
        => _pi.Execute(ref _kgsmPath, "--create-blueprint", "--help");

    /// <summary>
    /// Prints a list of all available blueprints
    /// </summary>
    public KgsmResult GetBlueprints()
        => _pi.Execute(ref _kgsmPath, "--blueprints");

    /// <summary>
    /// Create an instance of a blueprint
    /// </summary>
    /// <param name="blueprintName">Name of the blueprint to install</param>
    /// <param name="installDir">Optional installation directory</param>
    /// <param name="version">Optional version to install</param>
    /// <param name="id">Optional identifier used when creating the instance</param>
    public KgsmResult Install(string blueprintName, string? installDir = null, string? version = null, string? id = null) {

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
    public KgsmResult GetInstances()
        => _pi.Execute(ref _kgsmPath, "--instances");

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
    public KgsmResult IsActive(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--is-active");

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