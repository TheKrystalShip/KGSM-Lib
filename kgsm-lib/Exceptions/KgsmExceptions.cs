namespace TheKrystalShip.KGSM.Exceptions;

/// <summary>
/// Base exception for all KGSM-related exceptions.
/// </summary>
public class KgsmException : Exception
{
    /// <summary>
    /// Initializes a new instance of the KgsmException class.
    /// </summary>
    public KgsmException() { }

    /// <summary>
    /// Initializes a new instance of the KgsmException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public KgsmException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the KgsmException class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public KgsmException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a blueprint operation fails.
/// </summary>
public class BlueprintException : KgsmException
{
    /// <summary>
    /// Gets the name of the blueprint associated with the exception.
    /// </summary>
    public string? BlueprintName { get; }

    /// <summary>
    /// Initializes a new instance of the BlueprintException class.
    /// </summary>
    public BlueprintException() { }

    /// <summary>
    /// Initializes a new instance of the BlueprintException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BlueprintException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the BlueprintException class with a specified error message and blueprint name.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="blueprintName">The name of the blueprint associated with the exception.</param>
    public BlueprintException(string message, string blueprintName) : base(message)
    {
        BlueprintName = blueprintName;
    }

    /// <summary>
    /// Initializes a new instance of the BlueprintException class with a specified error message, blueprint name, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="blueprintName">The name of the blueprint associated with the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public BlueprintException(string message, string blueprintName, Exception innerException) : base(message, innerException)
    {
        BlueprintName = blueprintName;
    }
}

/// <summary>
/// Exception thrown when an instance operation fails.
/// </summary>
public class InstanceException : KgsmException
{
    /// <summary>
    /// Gets the name of the instance associated with the exception.
    /// </summary>
    public string? InstanceName { get; }

    /// <summary>
    /// Initializes a new instance of the InstanceException class.
    /// </summary>
    public InstanceException() { }

    /// <summary>
    /// Initializes a new instance of the InstanceException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InstanceException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the InstanceException class with a specified error message and instance name.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="instanceName">The name of the instance associated with the exception.</param>
    public InstanceException(string message, string instanceName) : base(message)
    {
        InstanceName = instanceName;
    }

    /// <summary>
    /// Initializes a new instance of the InstanceException class with a specified error message, instance name, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="instanceName">The name of the instance associated with the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public InstanceException(string message, string instanceName, Exception innerException) : base(message, innerException)
    {
        InstanceName = instanceName;
    }
}

/// <summary>
/// Exception thrown when a backup operation fails.
/// </summary>
public class BackupException : InstanceException
{
    /// <summary>
    /// Gets the name of the backup associated with the exception.
    /// </summary>
    public string? BackupName { get; }

    /// <summary>
    /// Initializes a new instance of the BackupException class.
    /// </summary>
    public BackupException() { }

    /// <summary>
    /// Initializes a new instance of the BackupException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BackupException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the BackupException class with a specified error message, instance name, and backup name.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="instanceName">The name of the instance associated with the exception.</param>
    /// <param name="backupName">The name of the backup associated with the exception.</param>
    public BackupException(string message, string instanceName, string backupName) : base(message, instanceName)
    {
        BackupName = backupName;
    }

    /// <summary>
    /// Initializes a new instance of the BackupException class with a specified error message, instance name, backup name, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="instanceName">The name of the instance associated with the exception.</param>
    /// <param name="backupName">The name of the backup associated with the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public BackupException(string message, string instanceName, string backupName, Exception innerException) : base(message, instanceName, innerException)
    {
        BackupName = backupName;
    }
}

/// <summary>
/// Exception thrown when a socket operation fails.
/// </summary>
public class SocketException : KgsmException
{
    /// <summary>
    /// Gets the path of the socket associated with the exception.
    /// </summary>
    public string? SocketPath { get; }

    /// <summary>
    /// Initializes a new instance of the SocketException class.
    /// </summary>
    public SocketException() { }

    /// <summary>
    /// Initializes a new instance of the SocketException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SocketException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the SocketException class with a specified error message and socket path.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="socketPath">The path of the socket associated with the exception.</param>
    public SocketException(string message, string socketPath) : base(message)
    {
        SocketPath = socketPath;
    }

    /// <summary>
    /// Initializes a new instance of the SocketException class with a specified error message, socket path, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="socketPath">The path of the socket associated with the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public SocketException(string message, string socketPath, Exception innerException) : base(message, innerException)
    {
        SocketPath = socketPath;
    }
}
