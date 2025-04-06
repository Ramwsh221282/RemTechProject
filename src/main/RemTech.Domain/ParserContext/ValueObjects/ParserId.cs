namespace RemTech.Domain.ParserContext.ValueObjects;

public readonly record struct ParserId
{
    public Guid Id { get; }

    public ParserId() => Id = Guid.Empty;

    public ParserId(Guid id) => Id = id;

    public static ParserId Empty() => new();

    public static ParserId New() => new(Guid.NewGuid());

    public static ParserId Dedicated(Guid id) => new(id);
}
