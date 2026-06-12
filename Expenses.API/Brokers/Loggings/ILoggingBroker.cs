namespace Expenses.API.Brokers.Loggings;

/// <summary>
/// Contract for logging operations.
/// </summary>
public interface ILoggingBroker
{
    /// <summary>
    /// Log information message
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    void LogInformation(string message);
    
    /// <summary>
    /// Log trace message
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    void LogTrace(string message);
    
    /// <summary>
    /// Log debug message
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    void LogDebug(string message);
    
    /// <summary>
    /// Log warning message
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    void LogWarning(string message);
    
    /// <summary>
    /// Log error message
    /// </summary>
    /// <param name="exception">The exception to be logged.</param>
    void LogError(Exception exception);
    
    /// <summary>
    /// Log critical message
    /// </summary>
    /// <param name="exception">The exception to be logged.</param>
    void LogCritical(Exception exception);
}