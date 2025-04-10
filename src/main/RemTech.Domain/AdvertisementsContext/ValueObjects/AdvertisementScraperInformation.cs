using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.AdvertisementsContext.ValueObjects;

public sealed record AdvertisementScraperInformation
{
    public const int MAX_SCRAPER_NAME_LENGTH = 100;
    public string SourceUrl { get; }
    public string ScraperName { get; }
    public string PublishedBy { get; }

    private AdvertisementScraperInformation(
        string sourceUrl,
        string scraperName,
        string publishedBy
    ) => (SourceUrl, ScraperName, PublishedBy) = (sourceUrl, scraperName, publishedBy);

    public static Result<AdvertisementScraperInformation> Create(
        string? sourceUrl,
        string? scraperInformation,
        string? publishedBy
    )
    {
        if (
            string.IsNullOrWhiteSpace(sourceUrl)
            || string.IsNullOrWhiteSpace(scraperInformation)
            || string.IsNullOrWhiteSpace(publishedBy)
        )
            return ErrorFactory.EmptyOrNull(nameof(AdvertisementScraperInformation));

        if (scraperInformation.Length > MAX_SCRAPER_NAME_LENGTH)
            return new Error($"Информация о скрапере превышает длину {MAX_SCRAPER_NAME_LENGTH}");

        return new AdvertisementScraperInformation(sourceUrl, scraperInformation, publishedBy);
    }
}
