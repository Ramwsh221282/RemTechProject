using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.TransportTypes;

public sealed record TransportType
{
    public string Name { get; }
    public string Link { get; }

    private TransportType(string name, string link)
    {
        Name = name;
        Link = link;
    }

    public static Result<TransportType> Create(string? name, string? link)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Error("Cannot create transport type without type name provided");

        if (string.IsNullOrWhiteSpace(link))
            return new Error("Cannot create transport type filter without link provided");

        return new TransportType(name, link);
    }
}
