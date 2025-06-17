using TheKrystalShip.KGSM.Core.Models;

namespace TheKrystalShip.KGSM.Core.Interfaces;

/// <summary>
/// Interface for executing processes.
/// </summary>
public interface IProcessRunner
{
    /// <summary>
    /// Executes a command with the specified arguments.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="args">Arguments to pass to the command.</param>
    /// <returns>Result of the command execution.</returns>
    ProcessResult Execute(string command, params string[] args);
}
