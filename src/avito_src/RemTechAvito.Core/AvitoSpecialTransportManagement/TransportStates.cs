using RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;
using RemTechCommon;
using RemTechCommon.Utils.GuidUtils;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement;

public sealed class TransportStates
{
    public AvitoSpecialTransportState State { get; }
    public TransportStateId Id { get; }

    public TransportStates(AvitoSpecialTransportState state)
    {
        State = state;
        Id = new TransportStateId(new RandomGuidGenerator());
    }
}
