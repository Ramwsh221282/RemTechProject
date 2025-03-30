using System.Diagnostics;
using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;

namespace RemTech.MongoDb.Service.Features.AdvertisementsManagement.AdvertisementQuerying;

public sealed class AdvertisementQueryBuilder
    : BaseQueryBuilder<Advertisement>,
        IQueryBuilder<AdvertisementQueryPayload, Advertisement>
{
    private AdvertisementQueryPayload? _payload;
    private readonly AdvertisementQueryBuilderWithFilters _withFilters = new();
    private readonly AdvertisementQueryBuilderWithNoFilters _noFilters = new();

    public void SetPayload(AdvertisementQueryPayload payload) => _payload = payload;

    public FilterDefinition<Advertisement> Build()
    {
        if (_payload == null)
            return Empty;
        IQueryBuilder<AdvertisementQueryPayload, Advertisement> builder = GetAppropriateBuilder(_payload);
        builder.SetPayload(_payload);
        return builder.Build();
    }

    private IQueryBuilder<AdvertisementQueryPayload, Advertisement> GetAppropriateBuilder(
        AdvertisementQueryPayload payload
    ) =>
        payload switch
        {
            AdvertisementQueryPayloadWithFilters => _withFilters,
            AdvertisementQueryPayloadWithoutFilters => _noFilters,
            _ => throw new UnreachableException(
                "Unsupported advertisement query payload filter types."
            ),
        };
}
