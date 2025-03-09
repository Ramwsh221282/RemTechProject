namespace AvitoParser.PDK.Models.ValueObjects;

public record ScrapedAdvertisementDate
{
    public DateTime Date { get; }

    public static ScrapedAdvertisementDate Default => new(DateTime.MinValue);

    public ScrapedAdvertisementDate(DateTime date)
    {
        Date = date;
    }
}
