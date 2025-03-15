namespace RemTechCommon.Utils.Converters;

public static class DateConverters
{
    public static long ToUnixFromDateOnly(this DateOnly date) =>
        new DateTimeOffset(
            DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
        ).ToUnixTimeMilliseconds();

    public static long ToUnixFromDateTime(this DateTime date) =>
        new DateTimeOffset(
            DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc)
        ).ToUnixTimeMilliseconds();

    public static DateOnly FromUnixToDateOnly(this long unixDate) =>
        DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(unixDate).UtcDateTime);

    public static DateTime FromUnixToDateTime(this long unixDate) =>
        DateTimeOffset.FromUnixTimeMilliseconds(unixDate).UtcDateTime;
}
