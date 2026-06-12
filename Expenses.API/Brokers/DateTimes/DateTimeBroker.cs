using AutoInject.Attributes.TransientAttributes;

namespace Expenses.API.Brokers.DateTimes;

/// <summary>
/// Class responsible for providing date and time information.
/// </summary>
[Transient(typeof(IDateTimeBroker))]
public class DateTimeBroker : IDateTimeBroker
{
    /// <inheritdoc/>
    public DateTimeOffset GetCurrentDateTimeOffset() => DateTimeOffset.UtcNow;
}