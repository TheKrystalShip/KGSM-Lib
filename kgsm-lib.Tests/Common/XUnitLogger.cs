using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace TheKrystalShip.KGSM.Tests.Common;

/// <summary>
/// Redirects Xunit test output to the logger.
/// </summary>
public class XUnitLogger<T> : ILogger<T>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _categoryName;

    /// <summary>
    /// Initializes a new instance of the <see cref="XUnitLogger{T}"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper to write to.</param>
    public XUnitLogger(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _categoryName = typeof(T).FullName ?? "Unknown";
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState _) where TState : notnull
    {
        return new NoopDisposable();
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        try
        {
            _testOutputHelper.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_categoryName}: {formatter(state, exception)}");
            
            if (exception != null)
            {
                _testOutputHelper.WriteLine($"Exception: {exception}");
            }
        }
        catch (Exception)
        {
            // Ignore exceptions from the test output helper
        }
    }

    private class NoopDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}

/// <summary>
/// Factory to create XUnit loggers.
/// </summary>
public class XUnitLoggerFactory : ILoggerFactory
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="XUnitLoggerFactory"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper to write to.</param>
    public XUnitLoggerFactory(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLogger<object>(_testOutputHelper);
    }

    /// <inheritdoc />
    public void AddProvider(ILoggerProvider provider)
    {
    }
}
