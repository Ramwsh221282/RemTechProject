using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportFilters;

public sealed record AvitoSpecialTransportState
{
    public string State { get; }

    private AvitoSpecialTransportState(string state) => State = state;

    public static Result<AvitoSpecialTransportState> Create(string? state) =>
        string.IsNullOrWhiteSpace(state)
            ? new Error("Special transport state is required")
            : new AvitoSpecialTransportState(state);
}
