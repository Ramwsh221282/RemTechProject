using RemTechCommon;
using RemTechCommon.Utils.GuidUtils;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record TransportStateId
{
    public Guid Id { get; }

    public TransportStateId(IGuidGenerationStrategy strategy) => Id = strategy.Generate();
}
