using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Services;

namespace TheKrystalShip.KGSM.Extensions;

/// <summary>
/// Extension methods for configuring KGSM services in an IServiceCollection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds KGSM services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="kgsmPath">The path to the KGSM executable.</param>
    /// <param name="socketPath">The path to the KGSM Unix socket.</param>
    /// <returns>The IServiceCollection so that additional calls can be chained.</returns>
    public static IServiceCollection AddKgsmServices(this IServiceCollection services, string kgsmPath, string socketPath)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(kgsmPath, nameof(kgsmPath));
        ArgumentNullException.ThrowIfNull(socketPath, nameof(socketPath));
        
        // Register process runner
        services.AddTransient<IProcessRunner, ProcessRunner>();

        // Register socket client
        services.AddSingleton<IUnixSocketClient>(provider => 
            new UnixSocketClient(socketPath, provider.GetRequiredService<ILogger<UnixSocketClient>>()));

        // Register event service
        services.AddSingleton<IEventService, EventService>();
        
        // Register blueprint service
        services.AddTransient<IBlueprintService>(provider => 
            new BlueprintService(
                provider.GetRequiredService<IProcessRunner>(), 
                kgsmPath, 
                provider.GetRequiredService<ILogger<BlueprintService>>()));
        
        // Register instance service
        services.AddTransient<IInstanceService>(provider => 
            new InstanceService(
                provider.GetRequiredService<IProcessRunner>(), 
                kgsmPath, 
                provider.GetRequiredService<ILogger<InstanceService>>()));
        
        // Register main client
        services.AddSingleton<IKgsmClient>(provider => 
            new KgsmClient(
                kgsmPath,
                provider.GetRequiredService<IProcessRunner>(),
                provider.GetRequiredService<IBlueprintService>(),
                provider.GetRequiredService<IInstanceService>(),
                provider.GetRequiredService<IEventService>(),
                provider.GetRequiredService<ILogger<KgsmClient>>()));

        return services;
    }

    /// <summary>
    /// Adds KGSM services with the specified configuration action.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configureOptions">Action to configure the KGSM options.</param>
    /// <returns>The IServiceCollection so that additional calls can be chained.</returns>
    public static IServiceCollection AddKgsmServices(this IServiceCollection services, Action<KgsmOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configureOptions, nameof(configureOptions));
        
        var options = new KgsmOptions();
        configureOptions(options);
        
        return AddKgsmServices(services, options.KgsmPath, options.SocketPath);
    }
}

/// <summary>
/// Options for configuring KGSM services.
/// </summary>
public class KgsmOptions
{
    /// <summary>
    /// Gets or sets the path to the KGSM executable.
    /// </summary>
    public string KgsmPath { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the path to the KGSM Unix socket.
    /// </summary>
    public string SocketPath { get; set; } = string.Empty;
}
