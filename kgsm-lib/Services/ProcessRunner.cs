using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Core.Models;

namespace TheKrystalShip.KGSM.Services;

/// <summary>
/// Implementation of the IProcessRunner interface for executing processes.
/// </summary>
public class ProcessRunner : IProcessRunner
{
    private readonly ILogger<ProcessRunner> _logger;

    /// <summary>
    /// Initializes a new instance of the ProcessRunner class.
    /// </summary>
    /// <param name="logger">The logger to use for logging.</param>
    public ProcessRunner(ILogger<ProcessRunner> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public ProcessResult Execute(string command, params string[] args)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        
        string arguments = string.Join(" ", args);
        _logger.LogDebug("Executing command: {Command} {Arguments}", command, arguments);

        ProcessStartInfo runningInfo = new()
        {
            FileName = command,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        Process? process;

        try
        {
            process = Process.Start(runningInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start process: {Command} {Arguments}", command, arguments);
            return new ProcessResult(1, string.Empty, ex.Message);
        }

        if (process is null)
        {
            _logger.LogError("Process failed to start: {Command} {Arguments}", command, arguments);
            return new ProcessResult(1, string.Empty, "Process failed to start");
        }

        process.WaitForExit();

        int exitCode = process.ExitCode;
        string stdout = process.StandardOutput.ReadToEnd().Trim() ?? string.Empty;
        string stderr = string.Empty;

        try
        {
            stderr = process.StandardError.ReadToEnd();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read standard error: {Command} {Arguments}", command, arguments);
        }

        if (exitCode != 0)
        {
            _logger.LogWarning("Command failed with exit code {ExitCode}: {Command} {Arguments}", exitCode, command, arguments);
            _logger.LogDebug("Command stderr: {Stderr}", stderr);
        }
        else
        {
            _logger.LogDebug("Command succeeded: {Command} {Arguments}", command, arguments);
        }

        return new ProcessResult(exitCode, stdout, stderr);
    }
}
