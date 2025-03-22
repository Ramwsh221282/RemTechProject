namespace RemTech.MongoDb.Service.Features.AdvertisementsSinking;

public sealed class SinkingAdvertisementValidator
{
    public bool Validate(SinkingAdvertisement advertisement)
    {
        if (advertisement.AdvertisementId <= 0)
            return false;

        if (string.IsNullOrWhiteSpace(advertisement.Address))
            return false;

        if (string.IsNullOrWhiteSpace(advertisement.Description))
            return false;

        if (string.IsNullOrWhiteSpace(advertisement.Title))
            return false;

        if (string.IsNullOrWhiteSpace(advertisement.Publisher))
            return false;

        if (string.IsNullOrWhiteSpace(advertisement.PriceExtra))
            advertisement.PriceExtra = "Без НДС";

        if (string.IsNullOrWhiteSpace(advertisement.SourceUrl))
            return false;

        if (string.IsNullOrWhiteSpace(advertisement.FromParser))
            return false;

        return !advertisement.Characteristics.Any(c =>
            string.IsNullOrWhiteSpace(c.Name) || string.IsNullOrWhiteSpace(c.Value)
        );
    }
}
