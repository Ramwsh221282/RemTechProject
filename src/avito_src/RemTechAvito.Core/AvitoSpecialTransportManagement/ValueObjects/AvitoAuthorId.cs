using RemTechCommon;
using RemTechCommon.Utils.GuidUtils;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record AvitoAuthorId
{
    public Guid Id { get; }

    public AvitoAuthorId(IGuidGenerationStrategy strategy) => Id = strategy.Generate();
}
