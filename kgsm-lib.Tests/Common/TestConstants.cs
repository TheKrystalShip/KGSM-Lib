namespace TheKrystalShip.KGSM.Tests.Common;

/// <summary>
/// Constants used throughout the test suite.
/// </summary>
public static class TestConstants
{
    /// <summary>
    /// The path to the KGSM executable.
    /// </summary>
    public const string KgsmPath = "/home/heisen/kgsm/kgsm.sh";
    
    /// <summary>
    /// The path to the KGSM socket.
    /// </summary>
    public const string KgsmSocketPath = "/home/heisen/kgsm/kgsm.sock";
    
    /// <summary>
    /// Installation directory for test instances.
    /// </summary>
    public const string TestInstallDir = "/tmp/kgsm-test";
    
    /// <summary>
    /// The test blueprints to use.
    /// </summary>
    public static readonly string[] TestBlueprints = ["factorio", "necesse", "terraria"];
    
    /// <summary>
    /// The timeout for test operations in milliseconds.
    /// </summary>
    public const int DefaultTimeoutMs = 15000;
    
    /// <summary>
    /// The delay for waiting between operations in milliseconds.
    /// </summary>
    public const int DefaultDelayMs = 2000;
}
