namespace TheKrystalShip.KGSM;

public class KgsmBlueprint()
{
    public string Name { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public int SteamAppId { get; set; } = 0;
    public bool SteamAccountRequired { get; set; } = false;
    public string LaunchBin { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public string LaunchBinSubdirectory { get; set; } = string.Empty;
    public string LaunchBinArgs { get; set; } = string.Empty;
    public string StopCommand { get; set; } = string.Empty;
    public string SaveCommand { get; set; } = string.Empty;
}