using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.AdvertisementsContext.ValueObjects;

public sealed record AdvertisementTextInformation
{
    public string Title { get; }
    public string Description { get; }

    private AdvertisementTextInformation(string title, string description) =>
        (Title, Description) = (title, description);

    public static Result<AdvertisementTextInformation> Create(string? title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            return ErrorFactory.EmptyOrNull(nameof(AdvertisementTextInformation));

        return new AdvertisementTextInformation(title, description);
    }
}
