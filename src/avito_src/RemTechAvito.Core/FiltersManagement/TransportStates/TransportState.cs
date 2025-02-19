using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.TransportStates;

public sealed record TransportState
{
    public string State { get; private set; }
    public DateOnly DateCreated { get; private set; }

    private TransportState(string state, DateOnly dateCreated)
    {
        State = state;
        DateCreated = dateCreated;
    }

    public static Result<TransportState> Create(string? state, DateOnly dateCreated) =>
        string.IsNullOrWhiteSpace(state)
            ? new Error("Transport state should be provided")
            : new TransportState(state, dateCreated);
}
