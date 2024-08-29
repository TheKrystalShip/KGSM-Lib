using TheKrystalShip.KGSM.Lib;

namespace TheKrystalShip.KGSM;

public record KgsmResult(int ExitCode, string? Stdout = null, string? Stderr = null)
{
    public KgsmResult(ProcessResult pr) : this(pr.ExitCode, pr.Stdout, pr.Stderr)
    {
    }

    public static implicit operator ProcessResult(KgsmResult x) => x;
    public static implicit operator KgsmResult(ProcessResult x) => new(x);
}
