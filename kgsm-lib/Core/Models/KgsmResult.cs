namespace TheKrystalShip.KGSM.Core.Models;

/// <summary>
/// Represents the result of a KGSM command execution.
/// </summary>
/// <param name="ExitCode">The exit code of the command.</param>
/// <param name="Stdout">The standard output of the command.</param>
/// <param name="Stderr">The standard error output of the command.</param>
public record KgsmResult(int ExitCode, string Stdout = "", string Stderr = "")
{
    /// <summary>
    /// Initializes a new instance of the KgsmResult class from a ProcessResult.
    /// </summary>
    /// <param name="pr">The ProcessResult to convert from.</param>
    public KgsmResult(ProcessResult pr) : this(pr.ExitCode, pr.Stdout, pr.Stderr)
    {
    }

    /// <summary>
    /// Implicitly converts a ProcessResult to a KgsmResult.
    /// </summary>
    /// <param name="x">The ProcessResult to convert.</param>
    public static implicit operator ProcessResult(KgsmResult x) => new(x.ExitCode, x.Stdout, x.Stderr);

    /// <summary>
    /// Implicitly converts a KgsmResult to a ProcessResult.
    /// </summary>
    /// <param name="x">The KgsmResult to convert.</param>
    public static implicit operator KgsmResult(ProcessResult x) => new(x);

    /// <summary>
    /// Returns whether the command was successful.
    /// </summary>
    public bool IsSuccess => ExitCode == 0;

    /// <summary>
    /// Returns whether the command failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;
}
