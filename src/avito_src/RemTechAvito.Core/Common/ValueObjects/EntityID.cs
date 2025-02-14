namespace RemTechAvito.Core.Common.ValueObjects;

public sealed record EntityID
{
    public Guid Id { get; }

    private EntityID(Guid id) => Id = id;

    public static EntityID New() => new EntityID(Guid.NewGuid());

    public static EntityID Empty() => new EntityID(Guid.Empty);

    public static EntityID Existing(Guid id) => new EntityID(id);
}
