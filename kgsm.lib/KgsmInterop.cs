using _pi = TheKrystalShip.KGSM.Lib.ProcessIntrop;

namespace TheKrystalShip.KGSM;

public class KgsmInterop(string kgsmPath)
{
    private string _kgsmPath = kgsmPath;

    public KgsmResult GetBlueprints()
        => _pi.Execute(ref _kgsmPath, "--blueprints");

    public KgsmResult GetInstances()
        => _pi.Execute(ref _kgsmPath, "--instances");

    public KgsmResult Start(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--start");

    public KgsmResult Stop(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--stop");

    public KgsmResult Restart(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--restart");

    public KgsmResult Status(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--status");

    public KgsmResult Info(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--info");

    public KgsmResult IsActive(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--is-active");

    public KgsmResult GetLogs(string instance)
        => _pi.Execute(ref _kgsmPath, "--instance", instance, "--logs");

    public KgsmResult GetIp()
        => _pi.Execute(ref _kgsmPath, "--ip");

    /// <summary>
    /// Execute Ad-Hoc commands
    /// Useful if the command you're trying to execute hasn't been mapped
    /// by KgsmInterop.
    /// </summary>
    /// <param name="args">Arguments to send to KGSM</param>
    /// <returns>KgsmResult</returns>
    public KgsmResult AdHoc(params string[] args)
        => _pi.Execute(ref _kgsmPath, args);
}
