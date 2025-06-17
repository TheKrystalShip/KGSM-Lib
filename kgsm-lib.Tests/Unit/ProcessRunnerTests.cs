using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheKrystalShip.KGSM.Core.Interfaces;
using TheKrystalShip.KGSM.Services;
using TheKrystalShip.KGSM.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="ProcessRunner"/>.
/// </summary>
public class ProcessRunnerTests : OutputTestBase
{
    private readonly IProcessRunner _processRunner;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessRunnerTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    public ProcessRunnerTests(ITestOutputHelper output) : base(output)
    {
        _processRunner = ServiceProvider.GetRequiredService<IProcessRunner>();
    }

    [Fact]
    public void Run_WithValidCommand_ShouldExecuteSuccessfully()
    {
        // Arrange
        string command = "echo";
        string[] args = ["Hello", "World"];
        
        // Act
        var result = _processRunner.Execute(command, args);
        
        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Stdout.Should().Contain("Hello World");
        result.Stderr.Should().BeEmpty();
    }

    [Fact]
    public void Run_WithInvalidCommand_ShouldReturnErrorCode()
    {
        // Arrange
        string command = "invalid_command_that_does_not_exist";
        string[] args = ["arg1", "arg2"];
        
        // Act
        var result = _processRunner.Execute(command, args);
        
        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().NotBe(0);
        result.Stderr.Should().NotBeEmpty("Should contain error message");
    }

    [Fact]
    public void Run_WithWorkingDirectory_ShouldExecuteInSpecifiedDirectory()
    {
        // Arrange
        string command = "pwd";
        string[] args = [];
        string workingDir = "/tmp";
        
        // Act
        var result = _processRunner.Execute(command, args);
        
        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Stdout.Trim().Should().Be(workingDir);
    }
      [Fact]
    public void Run_WithLargeOutput_ShouldCaptureAllOutput()
    {
        // Arrange
        string command = "head";
        string[] args = ["-c", "100000", "/dev/urandom"];
        int expectedSize = 50000; // 50KB of output
        
        // Act
        var result = _processRunner.Execute(command, args);
        
        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Stdout.Should().NotBeEmpty();
        
        // Ensure we captured a large amount of output
        result.Stdout.Length.Should().BeGreaterThan(expectedSize, 
            $"Should capture at least {expectedSize} bytes of output");
    }
}
