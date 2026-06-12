namespace Expenses.API.Brokers.DateTimes;

/// <summary>
/// Contract for a broker, responsible for providing the current date and time, which can be used for various
/// operations such as setting timestamps on entities, calculating time differences, etc.
/// </summary>
public interface IDateTimeBroker
{
    /// <summary>
    /// Gets the current date and time
    /// </summary>
    /// <returns>The current date and time</returns>
    DateTimeOffset GetCurrentDateTimeOffset();
}