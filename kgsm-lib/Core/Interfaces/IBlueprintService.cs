using TheKrystalShip.KGSM.Core.Models;

namespace TheKrystalShip.KGSM.Core.Interfaces;

/// <summary>
/// Interface for managing blueprints in KGSM.
/// </summary>
public interface IBlueprintService
{
    /// <summary>
    /// Gets a dictionary of all available blueprints.
    /// </summary>
    /// <returns>A dictionary of blueprint names to blueprint objects.</returns>
    Dictionary<string, Blueprint> GetAll();

    /// <summary>
    /// Creates a new blueprint with the specified configuration.
    /// </summary>
    /// <param name="blueprint">The blueprint configuration to create.</param>
    /// <returns>Result of the blueprint creation operation.</returns>
    KgsmResult Create(Blueprint blueprint);
}
