using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

public sealed record Publisher
{
    public string Name { get; }
    public string Description { get; }

    private Publisher(string name, string description) => (Name, Description) = (name, description);

    public static Result<Publisher> Create(string? name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Error("Publisher name is empty");
        if (string.IsNullOrWhiteSpace(description))
            return new Error("Publisher description is empty");
        return new Publisher(name, description);
    }
}
