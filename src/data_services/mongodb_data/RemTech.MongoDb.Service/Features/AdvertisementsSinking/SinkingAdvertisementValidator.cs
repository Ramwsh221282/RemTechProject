namespace RemTech.MongoDb.Service.Features.AdvertisementsSinking;

public sealed class SinkingAdvertisementValidator
{
    public bool Validate(SinkingAdvertisement advertisement)
    {
        if (advertisement.AdvertisementId <= 0)
        {
            Console.WriteLine($"ID is invalid. ID:{advertisement.AdvertisementId}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(advertisement.Address))
        {
            Console.WriteLine($"Address is invalid. Address: {advertisement.Address}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(advertisement.Description))
        {
            Console.WriteLine($"Description is invalid. Description: {advertisement.Description}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(advertisement.Title))
        {
            Console.WriteLine($"Title is invalid: Title: {advertisement.Title}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(advertisement.Publisher))
        {
            Console.WriteLine($"Publisher is invalid. Publisher: {advertisement.Publisher}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(advertisement.PriceExtra))
        {
            advertisement.PriceExtra = "Без НДС";
        }

        if (string.IsNullOrWhiteSpace(advertisement.SourceUrl))
        {
            Console.WriteLine($"Source url is invalid: {advertisement.SourceUrl}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(advertisement.FromParser))
            return false;

        if (
            advertisement.Characteristics.Any(c =>
                string.IsNullOrWhiteSpace(c.Name) || string.IsNullOrWhiteSpace(c.Value)
            )
        )
        {
            Console.WriteLine("Characteristics are invalid.");
            return false;
        }

        return true;
    }
}
