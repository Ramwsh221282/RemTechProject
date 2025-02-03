using RemTechCommon;
using RemTechCommon.Utils.GuidUtils;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public record AvitoSpecialTransportId
{
    public Guid Id { get; }

    public AvitoSpecialTransportId(IGuidGenerationStrategy strategy) => Id = strategy.Generate();
}
