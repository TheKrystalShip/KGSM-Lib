using FluentAssertions;
using TheKrystalShip.KGSM.Exceptions;
using TheKrystalShip.KGSM.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Unit;

/// <summary>
/// Unit tests for custom exceptions and error handling.
/// </summary>
public class ExceptionHandlingTests : OutputTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    public ExceptionHandlingTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void KgsmException_WithMessage_ShouldInitializeCorrectly()
    {
        // Arrange
        string errorMessage = "Test KGSM error";
        
        // Act
        var exception = new KgsmException(errorMessage);
        
        // Assert
        exception.Message.Should().Be(errorMessage);
    }

    [Fact]
    public void KgsmException_WithInnerException_ShouldInitializeCorrectly()
    {
        // Arrange
        string errorMessage = "Test KGSM error with inner exception";
        var innerException = new InvalidOperationException("Inner exception");
        
        // Act
        var exception = new KgsmException(errorMessage, innerException);
        
        // Assert
        exception.Message.Should().Be(errorMessage);
        exception.InnerException.Should().BeSameAs(innerException);
    }

    [Fact]
    public void SocketException_WithInnerException_ShouldInitializeCorrectly()
    {
        // Arrange
        string errorMessage = "Socket error";
        string socketPath = "/path/to/socket";
        var innerException = new InvalidOperationException("Inner exception");
        
        // Act
        var exception = new SocketException(errorMessage, socketPath, innerException);
        
        // Assert
        exception.Message.Should().Be(errorMessage);
        exception.InnerException.Should().BeSameAs(innerException);
        exception.SocketPath.Should().Be(socketPath);
    }

    [Fact]
    public void BlueprintException_WithBlueprintName_ShouldInitializeCorrectly()
    {
        // Arrange
        string blueprintName = "nonexistent-blueprint";
        string message = $"Blueprint '{blueprintName}' not found";
        
        // Act
        var exception = new BlueprintException(message, blueprintName);
        
        // Assert
        exception.Message.Should().Contain(blueprintName);
        exception.BlueprintName.Should().Be(blueprintName);
    }

    [Fact]
    public void InstanceException_WithInstanceName_ShouldInitializeCorrectly()
    {
        // Arrange
        string instanceName = "nonexistent-instance";
        string message = $"Instance '{instanceName}' not found";
        
        // Act
        var exception = new InstanceException(message, instanceName);
        
        // Assert
        exception.Message.Should().Contain(instanceName);
        exception.InstanceName.Should().Be(instanceName);
    }

    [Fact]
    public void BackupException_WithBackupName_ShouldInitializeCorrectly()
    {
        // Arrange
        string instanceName = "test-instance";
        string backupName = "test-backup";
        string message = $"Backup '{backupName}' not found for instance '{instanceName}'";
        
        // Act
        var exception = new BackupException(message, instanceName, backupName);
        
        // Assert
        exception.Message.Should().Contain(backupName);
        exception.InstanceName.Should().Be(instanceName);
        exception.BackupName.Should().Be(backupName);
    }

    [Fact]
    public void BlueprintException_WithInnerException_ShouldInitializeCorrectly()
    {
        // Arrange
        string blueprintName = "test-blueprint";
        string message = $"Error with blueprint '{blueprintName}'";
        var innerException = new InvalidOperationException("Inner exception");
        
        // Act
        var exception = new BlueprintException(message, blueprintName, innerException);
        
        // Assert
        exception.Message.Should().Be(message);
        exception.BlueprintName.Should().Be(blueprintName);
        exception.InnerException.Should().BeSameAs(innerException);
    }

    [Fact]
    public void NonexistentInstance_ShouldThrowKgsmException()
    {
        // Arrange
        string nonexistentInstance = $"nonexistent-{Guid.NewGuid():N}";
        
        // Act & Assert
        var action = () => KgsmClient.Instances.GetStatus(nonexistentInstance);
        action.Should().Throw<KgsmException>()
            .WithMessage($"*{nonexistentInstance}*");
    }

    [Fact]
    public void InvalidKgsmCommand_ShouldThrowKgsmException()
    {
        // Arrange
        string invalidCommand = $"invalid{Guid.NewGuid():N}";
        
        // Act & Assert
        var action = () => KgsmClient.AdHoc(invalidCommand, "arg1");
        action.Should().Throw<KgsmException>()
            .WithMessage($"*{invalidCommand}*");
    }
}
