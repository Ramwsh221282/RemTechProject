using System.Data;
using Dapper;
using RemTech.Domain.AdvertisementsContext;
using RemTech.Domain.AdvertisementsContext.ValueObjects;
using RemTech.Infrastructure.PostgreSql.Configuration;
using RemTech.Infrastructure.PostgreSql.NpgSqlCustomQueryParameters;

namespace SharedParsersLibrary.DatabaseSinking.Queries;

public sealed class InsertAdvertisementQuery(ConnectionStringFactory factory)
{
    private const string Sql = """
        INSERT 
        INTO advertisements (id, price_extra, price_value, published_by, scraper_name, source_url, description, title, characteristics, photos)
        VALUES (@id, @price_extra, @price_value, @published_by, @scraper_name, @source_url, @description, @title, @characteristics, @photos)
        """;
    private readonly ConnectionStringFactory _factory = factory;

    public async Task Insert(Advertisement advertisement)
    {
        long id = advertisement.Id.Value;
        string priceExtra = advertisement.Price.Extra;
        long priceValue = advertisement.Price.Value;
        string publishedBy = advertisement.Scraper.PublishedBy;
        string scraperName = advertisement.Scraper.ScraperName;
        string sourceUrl = advertisement.Scraper.SourceUrl;
        string description = advertisement.Text.Description;
        string title = advertisement.Text.Title;
        AdvertisementCharacteristicsCollection ctxCol = advertisement.Characteristics;
        AdvertisementPhotoCollection photoCol = advertisement.Photos;
        JsonBTypeParameter<AdvertisementCharacteristicsCollection> ctxJsonb = new(ctxCol);
        JsonBTypeParameter<AdvertisementPhotoCollection> photoJsonb = new(photoCol);

        using IDbConnection connection = _factory.Create();
        await connection.ExecuteAsync(
            Sql,
            new
            {
                id = id,
                price_extra = priceExtra,
                price_value = priceValue,
                published_by = publishedBy,
                scraper_name = scraperName,
                source_url = sourceUrl,
                description = description,
                title = title,
                characteristics = ctxJsonb,
                photos = photoJsonb,
            }
        );
    }
}
