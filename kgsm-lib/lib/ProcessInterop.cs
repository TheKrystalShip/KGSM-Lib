using System.Diagnostics;

namespace TheKrystalShip.KGSM.Lib;

public class ProcessIntrop
{
    public static ProcessResult Execute(ref string command, params string[] args)
    {
        ProcessStartInfo runningInfo = new()
        {
            FileName = command,
            Arguments = string.Join(" ", args),
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        Process? process = Process.Start(runningInfo);

        if (process is null)
        {
            return new ProcessResult(1, string.Empty, "Process failed to start");
        }

        process.WaitForExit();

        int exitCode = process.ExitCode;
        string stdout = process.StandardOutput.ReadToEnd().Trim() ?? string.Empty;
        string stderr = string.Empty;

        // Random intermittent error that StandardError has not been redirected
        // when it clearly has...
        try
        {
            stderr = process.StandardError.ReadToEnd();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return new ProcessResult(exitCode, stdout, stderr);
    }
}