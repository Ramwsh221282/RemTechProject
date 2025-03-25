using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;

namespace RemTech.MongoDb.Service.Features.AdvertisementsManagement.AdvertisementQuerying;

public record AdvertisementQueryPayload(
    long? AdvertisementId = null,
    string? Title = null,
    string? Description = null,
    long? Price = null,
    string? PriceExtra = null,
    string? ServiceName = null,
    string? SourceUrl = null,
    string? Publisher = null,
    string? Address = null,
    DateTime? CreatedAt = null,
    DateTime? AdvertisementDate = null,
    AdvertisementCharacteristicsQueryPayload[]? Characteristics = null
) : IQueryBuilderPayload<Advertisement>
{
    public bool IsPayloadEmpty =>
        AdvertisementId == null
        && Title == null
        && Description == null
        && Price == null
        && PriceExtra == null
        && ServiceName == null
        && SourceUrl == null
        && Publisher == null
        && Address == null
        && CreatedAt == null
        && AdvertisementDate == null
        && Characteristics == null;
}

public sealed record AdvertisementCharacteristicsQueryPayload(string Name, string Value);

public sealed record AdvertisementQueryPayloadWithFilters : AdvertisementQueryPayload
{
    public AdvertisementQueryPayloadWithFilters(AdvertisementQueryPayload payload)
        : base(
            AdvertisementId: payload.AdvertisementId,
            Title: payload.Title,
            Description: payload.Description,
            Price: payload.Price,
            PriceExtra: payload.PriceExtra,
            ServiceName: payload.ServiceName,
            SourceUrl: payload.SourceUrl,
            Publisher: payload.Publisher,
            Address: payload.Address,
            CreatedAt: payload.CreatedAt,
            AdvertisementDate: payload.AdvertisementDate,
            Characteristics: payload.Characteristics
        ) { }
}

public sealed record AdvertisementQueryPayloadWithoutFilters() : AdvertisementQueryPayload;

public static class AdvertisementQueryPayloadExtensions
{
    public static AdvertisementQueryPayload ResolveQueryPayload(this AdvertisementsQuery query)
    {
        return query.Payload.IsPayloadEmpty switch
        {
            true => new AdvertisementQueryPayloadWithoutFilters(),
            false => new AdvertisementQueryPayloadWithFilters(query.Payload),
        };
    }

    public static AdvertisementQueryPayload ResolveQueryPayload(
        this AdvertisementQueryPayload payload
    )
    {
        return payload.IsPayloadEmpty switch
        {
            true => new AdvertisementQueryPayloadWithoutFilters(),
            false => new AdvertisementQueryPayloadWithFilters(payload),
        };
    }
}
