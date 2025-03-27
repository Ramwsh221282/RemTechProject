using MongoDB.Bson;
using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Abstractions.BsonRegularExpressionUtils;
using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;
using RemTechCommon.Utils.Extensions;


namespace RemTech.MongoDb.Service.Features.AdvertisementsManagement.AdvertisementQuerying;

public sealed class AdvertisementQueryBuilderWithFilters
    : BaseQueryBuilder<Advertisement>,
        IQueryBuilder<AdvertisementQueryPayload, Advertisement>
{
    private AdvertisementQueryPayload? _payload;

    public void SetPayload(AdvertisementQueryPayload payload) => _payload = payload;

    public FilterDefinition<Advertisement> Build()
    {
        if (_payload == null || _payload.IsPayloadEmpty)
            return Empty;

        ApplyIdFilter();
        ApplyTitleFilter();
        ApplyDescriptionFilter();
        ApplyPriceFilter();
        ApplyPriceExtra();
        ApplyServiceName();
        ApplySourceUrl();
        ApplyPublisher();
        ApplyAddress();
        ApplyCreatedAt();
        ApplyCreatedAt();
        ApplyAdvertisementDate();
        ApplyCharacteristics();
        FilterDefinition<Advertisement> filter = Combined;
        return filter;
    }

    private void ApplyIdFilter()
    {
        long? id = _payload!.AdvertisementId;
        if (id == null)
            return;
        With(new BsonDocument("AdvertisementId", new BsonDocument("$eq", id.Value)));
    }

    private void ApplyTitleFilter()
    {
        string? title = _payload!.Title;
        if (string.IsNullOrWhiteSpace(title))
            return;
        BsonDocument filter = new BsonDocument("$regex", title.CleanString().CreateBsonRegularExpressionFromString());
        With(new BsonDocument("Title", filter));
    }

    private void ApplyDescriptionFilter()
    {
        string? description = _payload!.Description;
        if (string.IsNullOrWhiteSpace(description))
            return;
        BsonDocument filter = new BsonDocument("$regex", description.CleanString().CreateBsonRegularExpressionFromString());
        With(new BsonDocument("Description", filter));
    }

    private void ApplyPriceFilter()
    {
        long? price = _payload!.Price;
        if (price == null)
            return;
        With(new BsonDocument("Price", new BsonDocument("$eq", price.Value)));
    }

    private void ApplyPriceExtra()
    {
        string? extra = _payload!.PriceExtra;
        if (string.IsNullOrWhiteSpace(extra))
            return;
        BsonDocument filter = new BsonDocument("$regex", extra.CleanString().CreateBsonRegularExpressionFromString());
        With(new BsonDocument("PriceExtra", filter));
    }

    private void ApplyServiceName()
    {
        string? serviceName = _payload!.ServiceName;
        if (string.IsNullOrWhiteSpace(serviceName))
            return;
        With(new BsonDocument("ServiceName", new BsonDocument("$eq", serviceName)));
    }

    private void ApplySourceUrl()
    {
        string? sourceUrl = _payload!.SourceUrl;
        if (string.IsNullOrWhiteSpace(sourceUrl))
            return;
        With(new BsonDocument("SourceUrl", new BsonDocument("$eq", sourceUrl)));
    }

    private void ApplyPublisher()
    {
        string? publisher = _payload!.Publisher;
        if (string.IsNullOrWhiteSpace(publisher))
            return;
        With(new BsonDocument("Publisher", new BsonDocument($"eq", publisher)));
    }

    private void ApplyAddress()
    {
        string? address = _payload!.Address;
        if (string.IsNullOrWhiteSpace(address))
            return;
        BsonDocument filter = new BsonDocument("$regex", address.CleanString().CreateBsonRegularExpressionFromString());
        With(new BsonDocument("Address", filter));
    }

    private void ApplyCreatedAt()
    {
        DateTime? createdAt = _payload!.CreatedAt;
        if (createdAt == null)
            return;
        With(new BsonDocument("CreatedAt", new BsonDocument("$eq", createdAt.Value)));
    }

    private void ApplyAdvertisementDate()
    {
        DateTime? advertisementDate = _payload!.AdvertisementDate;
        if (advertisementDate == null)
            return;
        With(
            new BsonDocument("AdvertisementDate", new BsonDocument("$eq", advertisementDate.Value))
        );
    }

    private void ApplyCharacteristics()
    {
        AdvertisementCharacteristicsQueryPayload[]? ctx = _payload!.Characteristics;

        if (ctx == null || ctx.Length == 0)
            return;

        if (ctx.Any(c => string.IsNullOrEmpty(c.Name) || string.IsNullOrWhiteSpace(c.Value)))
            return;

        IEnumerable<AdvertisementCharacteristic> converted = ctx.Select(
            c => new AdvertisementCharacteristic(c.Name, c.Value)
        );

        foreach (var characteristic in converted)
        {
            BsonDocument matchFilter = new BsonDocument(
                "$elemMatch",
                new BsonDocument
                {
                    { "name", new BsonDocument("$eq", characteristic.Name) },
                    {
                        "value",
                        new BsonDocument(
                            "$regex",
                            characteristic.Value.CleanString().CreateBsonRegularExpressionFromString()
                        )
                    },
                }
            );
            BsonDocument finalFilter = new BsonDocument("Characteristics", matchFilter);
            With(finalFilter);
        }
    }
}
