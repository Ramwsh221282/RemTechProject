using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.TransportTypes;

public sealed record TransportType
{
    public string Name { get; private set; }
    public string Link { get; private set; }
    public DateOnly CreatedOn { get; private set; }

    private TransportType(string name, string link, DateOnly dateCreated)
    {
        Name = name;
        Link = link;
        CreatedOn = dateCreated;
    }

    public static Result<TransportType> Create(string? name, string? link, DateOnly dateCreated)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Error("Cannot create transport type without type name provided");

        if (string.IsNullOrWhiteSpace(link))
            return new Error("Cannot create transport type filter without link provided");

        return new TransportType(name, link, dateCreated);
    }
}
