using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.TransportStates;

public sealed record TransportState
{
    public string State { get; }

    private TransportState(string state) => State = state;

    public static Result<TransportState> Create(string? state) =>
        string.IsNullOrWhiteSpace(state)
            ? new Error("Transport state should be provided")
            : new TransportState(state);
}
