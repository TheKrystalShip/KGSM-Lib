using FluentAssertions;
using TheKrystalShip.KGSM.Core.Models;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="ProcessResult"/> and <see cref="KgsmResult"/>.
/// </summary>
public class ResultTests
{
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    public ResultTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ProcessResult_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var result = new ProcessResult(0, "stdout", "stderr");

        // Assert
        result.ExitCode.Should().Be(0);
        result.Stdout.Should().Be("stdout");
        result.Stderr.Should().Be("stderr");
    }

    [Fact]
    public void KgsmResult_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var result = new KgsmResult(0, "stdout", "stderr");

        // Assert
        result.ExitCode.Should().Be(0);
        result.Stdout.Should().Be("stdout");
        result.Stderr.Should().Be("stderr");
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void KgsmResult_FailureShouldBeDetectedCorrectly()
    {
        // Arrange & Act
        var result = new KgsmResult(1, "stdout", "stderr");

        // Assert
        result.ExitCode.Should().Be(1);
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void KgsmResult_ShouldInitializeFromProcessResult()
    {
        // Arrange
        var processResult = new ProcessResult(0, "stdout", "stderr");

        // Act
        var kgsmResult = new KgsmResult(processResult);

        // Assert
        kgsmResult.ExitCode.Should().Be(processResult.ExitCode);
        kgsmResult.Stdout.Should().Be(processResult.Stdout);
        kgsmResult.Stderr.Should().Be(processResult.Stderr);
    }

    [Fact]
    public void ProcessResult_ShouldImplicitlyConvertToKgsmResult()
    {
        // Arrange
        var processResult = new ProcessResult(0, "stdout", "stderr");

        // Act
        KgsmResult kgsmResult = processResult;

        // Assert
        kgsmResult.ExitCode.Should().Be(processResult.ExitCode);
        kgsmResult.Stdout.Should().Be(processResult.Stdout);
        kgsmResult.Stderr.Should().Be(processResult.Stderr);
    }

    [Fact]
    public void KgsmResult_ShouldImplicitlyConvertToProcessResult()
    {
        // Arrange
        var kgsmResult = new KgsmResult(0, "stdout", "stderr");

        // Act
        ProcessResult processResult = kgsmResult;

        // Assert
        processResult.ExitCode.Should().Be(kgsmResult.ExitCode);
        processResult.Stdout.Should().Be(kgsmResult.Stdout);
        processResult.Stderr.Should().Be(kgsmResult.Stderr);
    }
}
