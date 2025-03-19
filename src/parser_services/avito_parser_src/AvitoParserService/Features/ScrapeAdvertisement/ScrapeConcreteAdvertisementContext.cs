using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;

namespace AvitoParserService.Features.ScrapeAdvertisement;

public sealed class ScrapeConcreteAdvertisementContext
{
    public Option<ScrapedAdvertisement> ScrapedAdvertisement { get; private set; } =
        Option<ScrapedAdvertisement>.None();

    public void Initialize(ScrapedAdvertisement advertisement) =>
        ScrapedAdvertisement = Option<ScrapedAdvertisement>.Some(advertisement);

    public void SetTitle(string title) => ScrapedAdvertisement.Value.Title = title;

    public void SetDescription(string description) =>
        ScrapedAdvertisement.Value.Description = description;

    public void SetPublisher(string publisher) => ScrapedAdvertisement.Value.Publisher = publisher;

    public void SetPrice(long price) => ScrapedAdvertisement.Value.Price = price;

    public void SetCharacteristics(ScrapedCharacteristic[] characteristics) =>
        ScrapedAdvertisement.Value.Characteristics = characteristics;

    public void SetPriceExtra(string extra) => ScrapedAdvertisement.Value.PriceExtra = extra;

    public void SetAddress(string address) => ScrapedAdvertisement.Value.Address = address;

    public void SetDate(DateTime date) => ScrapedAdvertisement.Value.Published = date;
}
