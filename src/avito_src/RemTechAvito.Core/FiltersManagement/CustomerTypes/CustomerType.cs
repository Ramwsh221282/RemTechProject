using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.CustomerTypes;

public sealed record CustomerType
{
    public string Type { get; }

    private CustomerType(string type) => Type = type;

    public static Result<CustomerType> Create(string? type) =>
        string.IsNullOrWhiteSpace(type)
            ? new Error("Customer type cannot be empty")
            : new CustomerType(type);
}
