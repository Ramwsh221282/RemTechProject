using RemTechCommon;
using RemTechCommon.Utils.GuidUtils;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record TransportMarkId
{
    public Guid Id { get; }

    public TransportMarkId(IGuidGenerationStrategy strategy) => Id = strategy.Generate();
}
