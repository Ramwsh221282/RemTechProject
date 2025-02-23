namespace RemTechAvito.Core.ParserProfileManagement.ValueObjects;

public sealed record ParserProfileState
{
    public bool IsActive { get; }
    public string Description { get; }

    private ParserProfileState(bool isActive, string description)
    {
        IsActive = isActive;
        Description = description;
    }

    public static ParserProfileState CreateActive() => new ParserProfileState(true, "Активный");

    public static ParserProfileState CreateInactive() =>
        new ParserProfileState(false, "Неактивный");
}
