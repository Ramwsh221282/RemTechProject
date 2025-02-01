using RemTechAvito.Core.AvitoSpecialTransportFilters;
using RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;
using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement;

public sealed class TransportTypes
{
    public SpecialTransportType Type { get; }
    public TransportTypeId Id { get; }

    public TransportTypes(SpecialTransportType type)
    {
        Type = type;
        Id = new TransportTypeId(new RandomGuidGenerator());
    }
}
