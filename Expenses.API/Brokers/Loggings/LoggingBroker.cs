using AutoInject.Attributes.TransientAttributes;

namespace Expenses.API.Brokers.Loggings;

/// <summary>
/// Class responsible for logging operations.
/// </summary>
[Transient(typeof(ILoggingBroker))]
public class LoggingBroker : ILoggingBroker
{
    /// <summary>
    /// Logger instance for logging operations.
    /// </summary>
    private readonly ILogger<LoggingBroker> _logger;
    
    /// <summary>
    /// Constructor for LoggingBroker.
    /// </summary>
    /// <param name="logger">Logger instance for logging operations.</param>
    /// <exception cref="ArgumentNullException">Exception thrown when logger is null.</exception>
    public LoggingBroker(ILogger<LoggingBroker> logger) => 
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    
    /// <inheritdoc/>
    public void LogInformation(string message) => 
        _logger.LogInformation("{Message}", message);

    /// <inheritdoc/>
    public void LogTrace(string message) => 
        _logger.LogTrace("{Message}", message);

    /// <inheritdoc/>
    public void LogDebug(string message) => 
        _logger.LogDebug("{Message}", message);
    
    /// <inheritdoc/>
    public void LogWarning(string message) => 
        _logger.LogWarning("{Message}", message);
    
    /// <inheritdoc/>
    public void LogError(Exception exception) => 
        _logger.LogError(exception, "{ExceptionMessage}", exception.Message);
    
    /// <inheritdoc/>
    public void LogCritical(Exception exception) => 
        _logger.LogCritical(exception, "{ExceptionMessage}", exception.Message);
}