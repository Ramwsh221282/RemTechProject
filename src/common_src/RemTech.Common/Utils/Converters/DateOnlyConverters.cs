namespace RemTechCommon.Utils.Converters;

public static class DateOnlyConverters
{
    public static long ToUnix(this DateOnly date)
    {
        DateTime dateTimeUtc = DateTime.SpecifyKind(
            date.ToDateTime(TimeOnly.MinValue),
            DateTimeKind.Utc
        );
        return new DateTimeOffset(dateTimeUtc).ToUnixTimeMilliseconds();
    }

    public static DateOnly FromUnix(this long unixDate)
    {
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeMilliseconds(unixDate);
        return DateOnly.FromDateTime(dto.UtcDateTime);
    }
}
