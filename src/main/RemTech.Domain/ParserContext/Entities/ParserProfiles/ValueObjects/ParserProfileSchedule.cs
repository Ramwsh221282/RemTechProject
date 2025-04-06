using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;

public sealed record ParserProfileSchedule
{
    public long RepeatEveryUnixSeconds { get; }
    public long NextRunUnixSeconds { get; }

    public long RemainedUnixSeconds => NextRunUnixSeconds - RepeatEveryUnixSeconds;

    private ParserProfileSchedule(long repeatEveryUnixSeconds, long nextRunUnixSeconds)
    {
        RepeatEveryUnixSeconds = repeatEveryUnixSeconds;
        NextRunUnixSeconds = nextRunUnixSeconds;
    }

    public static ParserProfileSchedule CreateNonSet() => new(0, 0);

    public static Result<ParserProfileSchedule> CreateFromHour(int hour)
    {
        if (hour <= 0)
            return new Error("Некорректное значение часа.");

        long repeatEveryUnixSeconds = hour * 3600;
        long nextRunUnixSeconds = DateTimeOffset
            .UtcNow.AddSeconds(repeatEveryUnixSeconds)
            .ToUnixTimeSeconds();

        return new ParserProfileSchedule(repeatEveryUnixSeconds, nextRunUnixSeconds);
    }

    public static Result<ParserProfileSchedule> Create(
        long repeatEveryUnixSeconds,
        long nextRunUnixSeconds
    ) => new ParserProfileSchedule(repeatEveryUnixSeconds, nextRunUnixSeconds);
}
