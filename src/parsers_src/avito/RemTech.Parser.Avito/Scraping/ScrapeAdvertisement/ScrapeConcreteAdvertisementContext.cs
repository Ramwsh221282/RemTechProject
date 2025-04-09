using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Avito.Scraping.ScrapeAdvertisement;

public sealed class ScrapeConcreteAdvertisementContext
{
    public Option<ScrapedAdvertisement> ScrapedAdvertisement { get; private set; } =
        Option<ScrapedAdvertisement>.None();

    public void Initialize(ScrapedAdvertisement advertisement) =>
        ScrapedAdvertisement = Option<ScrapedAdvertisement>.Some(advertisement);

    public void SetTitle(string title)
    {
        if (ScrapedAdvertisement.HasValue == false)
            return;

        ScrapedAdvertisement updated = ScrapedAdvertisement.Value with { Title = title };
        ScrapedAdvertisement = updated;
    }

    public void SetDescription(string description)
    {
        if (ScrapedAdvertisement.HasValue == false)
            return;

        ScrapedAdvertisement updated = ScrapedAdvertisement.Value with
        {
            Description = description,
        };
        ScrapedAdvertisement = updated;
    }

    public void SetPublisher(string publisher)
    {
        if (ScrapedAdvertisement.HasValue == false)
            return;

        ScrapedAdvertisement updated = ScrapedAdvertisement.Value with { Publisher = publisher };
        ScrapedAdvertisement = updated;
    }

    public void SetPrice(long price)
    {
        if (ScrapedAdvertisement.HasValue == false)
            return;

        ScrapedAdvertisement updated = ScrapedAdvertisement.Value with { Price = price };
        ScrapedAdvertisement = updated;
    }

    public void SetCharacteristics(ScrapedCharacteristic[] characteristics)
    {
        if (ScrapedAdvertisement.HasValue == false)
            return;

        ScrapedAdvertisement updated = ScrapedAdvertisement.Value with
        {
            Characteristics = characteristics,
        };
        ScrapedAdvertisement = updated;
    }

    public void SetPriceExtra(string extra)
    {
        if (ScrapedAdvertisement.HasValue == false)
            return;

        ScrapedAdvertisement updated = ScrapedAdvertisement.Value with { PriceExtra = extra };
        ScrapedAdvertisement = updated;
    }

    public void SetAddress(string address)
    {
        if (ScrapedAdvertisement.HasValue == false)
            return;

        ScrapedAdvertisement updated = ScrapedAdvertisement.Value with { Address = address };
        ScrapedAdvertisement = updated;
    }

    public void SetDate(DateTime date)
    {
        if (ScrapedAdvertisement.HasValue == false)
            return;

        ScrapedAdvertisement updated = ScrapedAdvertisement.Value with { Published = date };
        ScrapedAdvertisement = updated;
    }
}
