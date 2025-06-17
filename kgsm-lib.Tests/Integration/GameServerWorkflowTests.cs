using FluentAssertions;
using Microsoft.Extensions.Logging;
using TheKrystalShip.KGSM.Core.Models;
using TheKrystalShip.KGSM.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Integration;

/// <summary>
/// End-to-end integration test that exercises a complete game server workflow.
/// </summary>
public class GameServerWorkflowTests : OutputTestBase
{
    private readonly ILogger<GameServerWorkflowTests> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerWorkflowTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    public GameServerWorkflowTests(ITestOutputHelper output) : base(output)
    {
        _logger = LoggerFactory.CreateLogger<GameServerWorkflowTests>();
    }

    [Theory]
    [InlineData("factorio")]
    [InlineData("necesse")]
    [InlineData("terraria")]
    public void CompleteGameServerWorkflow_ShouldSucceed(string blueprintName)
    {
        // Arrange
        string instanceName = GetUniqueInstanceName(blueprintName);
        string installDir = GetUniqueInstallDir(instanceName);
        
        _logger.LogInformation("Starting workflow test for {Blueprint} with instance name {InstanceName}", 
            blueprintName, instanceName);

        try
        {
            // Step 1: Install the game server instance
            _logger.LogInformation("Step 1: Installing {Blueprint} instance", blueprintName);
            var installResult = KgsmClient.Instances.Install(blueprintName, installDir, name: instanceName);
            installResult.Should().NotBeNull();
            installResult.IsSuccess.Should().BeTrue($"Installation of {blueprintName} should succeed");
            
            // Step 2: Verify instance exists and has correct properties
            _logger.LogInformation("Step 2: Verifying instance exists");
            var instances = KgsmClient.Instances.GetAll();
            instances.Should().ContainKey(instanceName);
            
            var instance = instances[instanceName];
            instance.Blueprint.Should().Be(blueprintName);
            instance.Directory.Should().Be(installDir);
            
            // Step 3: Start the instance
            _logger.LogInformation("Step 3: Starting instance");
            var startResult = KgsmClient.Instances.Start(instanceName);
            startResult.Should().NotBeNull();
            startResult.IsSuccess.Should().BeTrue($"Starting {blueprintName} instance should succeed");
            
            // Step 4: Wait and verify instance is running
            _logger.LogInformation("Step 4: Verifying instance is running");
            Thread.Sleep(5000); // Give it time to fully start
            var isActive = KgsmClient.Instances.IsActive(instanceName);
            isActive.Should().BeTrue($"{blueprintName} instance should be active after starting");
            
            // Step 5: Get instance info
            _logger.LogInformation("Step 5: Getting instance info");
            var infoResult = KgsmClient.Instances.GetInfo(instanceName);
            infoResult.Should().NotBeNull();
            infoResult.IsSuccess.Should().BeTrue();
            infoResult.Stdout.Should().Contain(instanceName);
            infoResult.Stdout.Should().Contain(blueprintName);
            
            // Step 6: Check logs
            _logger.LogInformation("Step 6: Checking logs");
            var logsResult = KgsmClient.Instances.GetLogs(instanceName);
            logsResult.Should().NotBeNull();
            logsResult.IsSuccess.Should().BeTrue();
            logsResult.Stdout.Should().NotBeEmpty("Log file should contain content");
            
            // Step 7: Stop the instance
            _logger.LogInformation("Step 7: Stopping instance");
            var stopResult = KgsmClient.Instances.Stop(instanceName);
            stopResult.Should().NotBeNull();
            stopResult.IsSuccess.Should().BeTrue($"Stopping {blueprintName} instance should succeed");
            
            // Step 8: Wait and verify instance is stopped
            _logger.LogInformation("Step 8: Verifying instance is stopped");
            Thread.Sleep(2000); // Give it time to fully stop
            isActive = KgsmClient.Instances.IsActive(instanceName);
            isActive.Should().BeFalse($"{blueprintName} instance should be inactive after stopping");
            
            // Step 9: Check version info
            _logger.LogInformation("Step 9: Checking version info");
            var versionResult = KgsmClient.Instances.GetInstalledVersion(instanceName);
            versionResult.Should().NotBeNull();
            versionResult.IsSuccess.Should().BeTrue();
            versionResult.Stdout.Should().NotBeEmpty("Should return installed version");
            
            var latestResult = KgsmClient.Instances.GetLatestVersion(instanceName);
            latestResult.Should().NotBeNull();
            latestResult.IsSuccess.Should().BeTrue();
            latestResult.Stdout.Should().NotBeEmpty("Should return latest version");
            
            _logger.LogInformation("Installed version: {Installed}, Latest version: {Latest}", 
                versionResult.Stdout.Trim(), latestResult.Stdout.Trim());
            
            // Step 10: Create backup (if supported)
            _logger.LogInformation("Step 10: Creating backup");
            var backupResult = KgsmClient.AdHoc("backup", instanceName);
            
            if (backupResult.IsSuccess)
            {
                _logger.LogInformation("Backup created: {BackupOutput}", backupResult.Stdout.Trim());
            }
            else
            {
                _logger.LogWarning("Backup not supported for {Blueprint} or failed: {ErrorMessage}", 
                    blueprintName, backupResult.Stderr);
            }
        }
        finally
        {
            // Cleanup
            _logger.LogInformation("Cleaning up test instance");
            
            try
            {
                // Stop instance if running
                if (KgsmClient.Instances.IsActive(instanceName))
                {
                    KgsmClient.Instances.Stop(instanceName);
                    Thread.Sleep(2000); // Give it time to fully stop
                }
                
                // Uninstall test instance
                KgsmClient.Instances.Uninstall(instanceName);
                
                // Clean up test directory
                if (Directory.Exists(installDir))
                {
                    Directory.Delete(installDir, true);
                }
                
                _logger.LogInformation("Cleanup completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cleanup of test instance");
            }
        }
    }
}
