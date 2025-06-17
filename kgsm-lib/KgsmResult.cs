using TheKrystalShip.KGSM.Lib;

namespace TheKrystalShip.KGSM;

public record KgsmResult(int ExitCode, string Stdout = "", string Stderr = "")
{
    public KgsmResult(ProcessResult pr) : this(pr.ExitCode, pr.Stdout, pr.Stderr)
    {
    }

    // Allow conversion from ProcessResult to KgsmResult
    // and vice versa
    public static implicit operator ProcessResult(KgsmResult x) => x;
    public static implicit operator KgsmResult(ProcessResult x) => new(x);
}