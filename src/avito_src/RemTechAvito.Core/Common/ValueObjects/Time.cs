using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.Common.ValueObjects;

public sealed record Time
{
    public int Hours { get; }
    public int Minutes { get; }
    public int Seconds { get; }

    private Time(int hours, int minutes, int seconds)
    {
        Hours = hours;
        Minutes = minutes;
        Seconds = seconds;
    }

    public static Result<Time> Create(int hours, int minutes, int seconds)
    {
        if (hours < 0)
            return new Error("Hour cannot be negative");
        if (minutes < 0)
            return new Error("Minute cannot be negative");
        if (seconds < 0)
            return new Error("Second cannot be negative");
        return new Time(hours, minutes, seconds);
    }

    public static Time Create(TimeSpan timeSpan)
    {
        return new Time(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}
