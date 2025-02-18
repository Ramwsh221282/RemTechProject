using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.CustomerStates;

public sealed record CustomerState
{
    public string State { get; private set; }
    public DateOnly CreatedOn { get; private set; }

    private CustomerState(string state, DateOnly createdOn)
    {
        State = state;
        CreatedOn = createdOn;
    }

    public static Result<CustomerState> Create(string? state, DateOnly createdOn) =>
        string.IsNullOrWhiteSpace(state)
            ? new Error("Customer state cannot be empty")
            : new CustomerState(state, createdOn);
}
