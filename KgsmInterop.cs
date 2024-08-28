using TheKrystalShip.KGSM.Lib;

namespace TheKrystalShip.KGSM;

public class KgsmInterop
{
    private readonly ProcessIntrop _processIntrop;
    private readonly string _kgsmPath;

    public KgsmInterop(string kgsmPath)
    {
        _processIntrop = new();
        _kgsmPath = kgsmPath;
    }
}
