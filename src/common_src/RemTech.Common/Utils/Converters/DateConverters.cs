namespace RemTechCommon.Utils.Converters;

public static class DateConverters
{
    public static long ToUnixFromDateOnly(this DateOnly date) =>
        new DateTimeOffset(
            date.ToDateTime(TimeOnly.MinValue),
            TimeSpan.Zero
        ).ToUnixTimeMilliseconds();

    public static long ToUnixFromDateTime(this DateTime date) =>
        new DateTimeOffset(DateTime.SpecifyKind(date, DateTimeKind.Utc)).ToUnixTimeMilliseconds();

    public static DateOnly FromUnixToDateOnly(this long unixDate) =>
        DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(unixDate).UtcDateTime);

    public static DateTime FromUnixToDateTime(this long unixDate) =>
        DateTimeOffset.FromUnixTimeMilliseconds(unixDate).UtcDateTime;
}
