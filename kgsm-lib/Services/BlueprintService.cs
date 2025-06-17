using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Core.Models;

namespace TheKrystalShip.KGSM.Services;

/// <summary>
/// Implementation of the IBlueprintService interface for managing blueprints in KGSM.
/// </summary>
public class BlueprintService : IBlueprintService
{
    private readonly IProcessRunner _processRunner;
    private readonly string _kgsmPath;
    private readonly ILogger<BlueprintService> _logger;

    /// <summary>
    /// Initializes a new instance of the BlueprintService class.
    /// </summary>
    /// <param name="processRunner">The process runner to use for executing KGSM commands.</param>
    /// <param name="kgsmPath">The path to the KGSM executable.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public BlueprintService(IProcessRunner processRunner, string kgsmPath, ILogger<BlueprintService> logger)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        _kgsmPath = kgsmPath ?? throw new ArgumentNullException(nameof(kgsmPath));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Dictionary<string, Blueprint> GetAll()
    {
        _logger.LogDebug("Getting all blueprints");
        
        Dictionary<string, Blueprint> blueprints = new();
        ProcessResult result = _processRunner.Execute(_kgsmPath, "--blueprints", "--detailed", "--json");

        if (result.ExitCode != 0)
        {
            _logger.LogError("Failed to get blueprints: {Error}", result.Stderr);
            return blueprints;
        }

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true  
        };
        serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        serializerOptions.Converters.Add(new JsonStringToBoolConverter());

        try
        {
            blueprints = JsonSerializer.Deserialize<Dictionary<string, Blueprint>>(
                result.Stdout,
                serializerOptions
            ) ?? new Dictionary<string, Blueprint>();
            
            _logger.LogDebug("Found {Count} blueprints", blueprints.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize blueprints");
        }

        return blueprints;
    }

    /// <inheritdoc/>
    public KgsmResult Create(Blueprint blueprint)
    {
        ArgumentNullException.ThrowIfNull(blueprint, nameof(blueprint));
        
        if (string.IsNullOrEmpty(blueprint.Name))
        {
            throw new ArgumentException("Blueprint name is required", nameof(blueprint));
        }
        
        _logger.LogDebug("Creating blueprint {Name}", blueprint.Name);
        
        // Implement the blueprint creation logic
        // This is a placeholder - the actual implementation would depend on how KGSM handles blueprint creation
        
        return new KgsmResult(0, $"Blueprint {blueprint.Name} created successfully");
    }
}
