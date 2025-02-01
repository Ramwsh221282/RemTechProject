using RemTechAvito.Core.AvitoSpecialTransportFilters;
using RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;
using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement;

public sealed class TransportMarks
{
    public AvitoSpecialTransportMark Mark { get; }
    public TransportMarkId Id { get; }

    public TransportMarks(AvitoSpecialTransportMark mark)
    {
        Mark = mark;
        Id = new TransportMarkId(new RandomGuidGenerator());
    }
}
