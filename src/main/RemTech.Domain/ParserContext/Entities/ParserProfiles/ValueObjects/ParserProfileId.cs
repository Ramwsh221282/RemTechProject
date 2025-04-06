namespace RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;

public readonly record struct ParserProfileId
{
    public Guid Value { get; }

    public ParserProfileId()
    {
        Value = Guid.Empty;
    }

    public ParserProfileId(Guid id)
    {
        Value = id;
    }

    public static ParserProfileId New() => new(Guid.NewGuid());

    public static ParserProfileId Dedicated(Guid id) => new(id);
}
