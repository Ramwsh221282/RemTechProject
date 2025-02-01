using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record TransportMarkId
{
    public Guid Id { get; }

    public TransportMarkId(IGuidGenerationStrategy strategy) => Id = strategy.Generate();
}
