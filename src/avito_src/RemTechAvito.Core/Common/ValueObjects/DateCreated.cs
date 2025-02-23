namespace RemTechAvito.Core.Common.ValueObjects;

public sealed record DateCreated(DateOnly Date)
{
    public static DateCreated Current()
    {
        DateOnly date = DateOnly.FromDateTime(DateTime.Now);
        return new DateCreated(date);
    }

    public static DateCreated ConcreteDate(DateOnly date)
    {
        return new DateCreated(date);
    }
}
