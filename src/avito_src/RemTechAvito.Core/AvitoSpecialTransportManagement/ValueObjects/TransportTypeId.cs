using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record TransportTypeId
{
    public Guid Id { get; }

    public TransportTypeId(IGuidGenerationStrategy strategy) => Id = strategy.Generate();
}
