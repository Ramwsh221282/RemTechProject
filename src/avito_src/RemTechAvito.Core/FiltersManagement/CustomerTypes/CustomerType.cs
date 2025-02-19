using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.CustomerTypes;

public sealed record CustomerType
{
    public string Type { get; private set; }

    public DateOnly CreatedOn { get; private set; }

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
