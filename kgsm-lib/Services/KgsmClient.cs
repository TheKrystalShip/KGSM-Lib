using System.Text.Json;
using Microsoft.Extensions.Logging;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Core.Models;

namespace TheKrystalShip.KGSM.Services;

/// <summary>
/// Implementation of the IKgsmClient interface for interacting with KGSM.
/// </summary>
public class KgsmClient : IKgsmClient
{
    private readonly IProcessRunner _processRunner;
    private readonly string _kgsmPath;
    private readonly ILogger<KgsmClient> _logger;

    /// <inheritdoc/>
    public IBlueprintService Blueprints { get; }
    
    /// <inheritdoc/>
    public IInstanceService Instances { get; }
    
    /// <inheritdoc/>
    public IEventService Events { get; }

    /// <summary>
    /// Initializes a new instance of the KgsmClient class.
    /// </summary>
    /// <param name="kgsmPath">The path to the KGSM executable.</param>
    /// <param name="processRunner">The process runner to use for executing KGSM commands.</param>
    /// <param name="blueprintService">The blueprint service to use for managing blueprints.</param>
    /// <param name="instanceService">The instance service to use for managing instances.</param>
    /// <param name="eventService">The event service to use for handling events.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public KgsmClient(
        string kgsmPath,
        IProcessRunner processRunner,
        IBlueprintService blueprintService,
        IInstanceService instanceService,
        IEventService eventService,
        ILogger<KgsmClient> logger)
    {
        ArgumentNullException.ThrowIfNull(kgsmPath);

        _kgsmPath = kgsmPath;
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        Blueprints = blueprintService ?? throw new ArgumentNullException(nameof(blueprintService));
        Instances = instanceService ?? throw new ArgumentNullException(nameof(instanceService));
        Events = eventService ?? throw new ArgumentNullException(nameof(eventService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _logger.LogInformation("KgsmClient initialized with KGSM path: {KgsmPath}", _kgsmPath);
        
        // Initialize the event service
        Events.Initialize();
    }

    /// <inheritdoc/>
    public KgsmResult Help()
    {
        _logger.LogDebug("Getting help information");
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--help");
        
        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult HelpInteractive()
    {
        _logger.LogDebug("Getting interactive help information");
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--help", "--interactive");
        
        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult UpdateKgsm()
    {
        _logger.LogInformation("Updating KGSM");
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--update");
        
        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to update KGSM: {Error}", result.Stderr);
        }
        else
        {
            _logger.LogInformation("KGSM updated successfully");
        }
        
        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult GetIp()
    {
        _logger.LogDebug("Getting server IP address");
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--ip");
        
        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult GetVersion()
    {
        _logger.LogDebug("Getting KGSM version");
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--version");
        
        return new KgsmResult(result);
    }

    /// <inheritdoc/>
    public KgsmResult AdHoc(params string[] args)
    {
        _logger.LogDebug("Executing ad-hoc command with arguments: {Arguments}", string.Join(" ", args));
        
        ProcessResult result = _processRunner.Execute(_kgsmPath, args);
        
        return new KgsmResult(result);
    }
}
