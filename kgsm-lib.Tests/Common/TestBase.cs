using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Extensions;

namespace TheKrystalShip.KGSM.Tests.Common;

/// <summary>
/// Common base class for tests that provides the necessary services and configuration.
/// </summary>
public abstract class TestBase : IDisposable
{

    /// <summary>
    /// The service provider for dependency injection.
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;
    
    /// <summary>
    /// The KGSM client for interacting with KGSM.
    /// </summary>
    protected readonly IKgsmClient KgsmClient;
    
    /// <summary>
    /// The logger factory for creating loggers.
    /// </summary>
    protected readonly ILoggerFactory LoggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestBase"/> class.
    /// </summary>
    protected TestBase()
    {
        var services = new ServiceCollection();
        
        // Add logging
        services.AddLogging(builder => 
            builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        
        // Add KGSM services
        services.AddKgsmServices(TestConstants.KgsmPath, TestConstants.KgsmSocketPath);

        // Build service provider
        ServiceProvider = services.BuildServiceProvider();
        
        // Get services
        KgsmClient = ServiceProvider.GetRequiredService<IKgsmClient>();
        LoggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();

        // Ensure test directory exists
        Directory.CreateDirectory(TestConstants.TestInstallDir);
    }

    /// <summary>
    /// Generates a unique instance name for testing.
    /// </summary>
    /// <param name="blueprintName">The name of the blueprint to base the instance name on.</param>
    /// <returns>A unique instance name.</returns>
    protected string GetUniqueInstanceName(string blueprintName)
    {
        return $"{blueprintName}-test-{Guid.NewGuid().ToString()[..8]}";
    }

    /// <summary>
    /// Generates a unique installation directory for testing.
    /// </summary>
    /// <param name="instanceName">The name of the instance to create a directory for.</param>
    /// <returns>A unique installation directory path.</returns>
    protected string GetUniqueInstallDir(string instanceName)
    {
        string path = Path.Combine(TestConstants.TestInstallDir, instanceName);
        Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Cleans up resources used by the test.
    /// </summary>
    public virtual void Dispose()
    {
        // Cleanup logic here if needed
        GC.SuppressFinalize(this);
    }
}
