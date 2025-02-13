using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.TransportTypes;

public sealed record TransportType
{
    public string Name { get; }

    private TransportType(string name) => Name = name;

    public static Result<TransportType> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Error("Cannot create transport type");

        return new TransportType(name);
    }
}
