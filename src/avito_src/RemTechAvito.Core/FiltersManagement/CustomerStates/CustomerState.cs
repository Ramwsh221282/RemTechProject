using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.CustomerStates;

public sealed record CustomerState
{
    public string State { get; }

    private CustomerState(string state) => State = state;

    public static Result<CustomerState> Create(string? state) =>
        string.IsNullOrWhiteSpace(state)
            ? new Error("Customer state cannot be empty")
            : new CustomerState(state);
}
