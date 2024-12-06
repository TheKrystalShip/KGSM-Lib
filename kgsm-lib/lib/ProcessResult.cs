namespace TheKrystalShip.KGSM.Lib;

public record ProcessResult(int ExitCode, string Stdout = "", string Stderr = "");