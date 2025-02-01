using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportFilters;

public sealed record SpecialTransportType
{
    public string Type { get; }

    private SpecialTransportType(string type) => Type = type;

    public static Result<SpecialTransportType> Create(string? type) =>
        string.IsNullOrWhiteSpace(type)
            ? new Error("Special transport type cannot be empty")
            : new SpecialTransportType(type);
}
