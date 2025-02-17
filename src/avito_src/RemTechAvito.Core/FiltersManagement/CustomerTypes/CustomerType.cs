using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.CustomerTypes;

public sealed record CustomerType
{
    public string Type { get; }

    public DateOnly CreatedOn { get; }

    private CustomerType(string type, DateOnly createdOn)
    {
        Type = type;
        CreatedOn = createdOn;
    }

    public static Result<CustomerType> Create(string? type, DateOnly createdOn) =>
        string.IsNullOrWhiteSpace(type)
            ? new Error("Customer type cannot be empty")
            : new CustomerType(type, createdOn);
}
