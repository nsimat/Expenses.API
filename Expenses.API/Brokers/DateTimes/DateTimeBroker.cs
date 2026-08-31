using AutoInject.Attributes.TransientAttributes;

namespace Expenses.API.Brokers.DateTimes;

/// <summary>
/// Class responsible for providing date and time information.
/// </summary>
[Transient(typeof(IDateTimeBroker))]
public class DateTimeBroker : IDateTimeBroker
{
    /// <summary>
    /// Provides an abstraction for managing and accessing date and time, enabling easier testing and
    /// better control over time-dependent operations within the application.
    /// </summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Provides an implementation of the <see cref="IDateTimeBroker"/> interface to manage and access date and time
    /// functionality. Facilitates better testability and abstraction over time-based operations.
    /// </summary>
    public DateTimeBroker(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    
    /// <inheritdoc/>
    public DateTimeOffset GetCurrentDateTimeOffset() => _timeProvider.GetUtcNow();
}