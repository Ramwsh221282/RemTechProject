namespace RemTechAvito.Core.Common.ValueObjects;

public sealed record TransportAdvertisementID
{
    public Guid Id { get; }

    public TransportAdvertisementID(Guid id) => Id = id;
}
