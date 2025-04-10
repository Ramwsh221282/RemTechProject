using Dapper.FluentMap.Mapping;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.EntityMappings;

public sealed class AdvertisementEntityMap : EntityMap<AdvertisementDao>
{
    public AdvertisementEntityMap()
    {
        Map(ad => ad.Id).ToColumn("id");
        Map(ad => ad.PriceExtra).ToColumn("price_extra");
        Map(ad => ad.PriceValue).ToColumn("price_value");
        Map(ad => ad.PublishedBy).ToColumn("published_by");
        Map(ad => ad.ScraperName).ToColumn("scraper_name");
        Map(ad => ad.SourceUrl).ToColumn("source_url");
        Map(ad => ad.Description).ToColumn("description");
        Map(ad => ad.Title).ToColumn("title");
        Map(ad => ad.CharacteristicsJson).ToColumn("characteristics");
        Map(ad => ad.PhotosJson).ToColumn("photos");
        Map(ad => ad.Address).ToColumn("address");
    }
}
