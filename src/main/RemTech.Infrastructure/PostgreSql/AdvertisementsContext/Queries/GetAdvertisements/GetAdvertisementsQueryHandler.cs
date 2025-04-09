using System.Data;
using Dapper;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisements.Specification;
using RemTech.Infrastructure.PostgreSql.Configuration;
using RemTech.Shared.SDK.CqrsPattern.Queries;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisements;

public sealed class GetAdvertisementsQueryHandler(ConnectionStringFactory factory)
    : IQueryHandler<GetAdvertisementsQuery, AdvertisementDao[]>
{
    private const string Sql = "SELECT * FROM advertisements WHERE 1=1 ";

    private readonly ConnectionStringFactory _factory = factory;

    public async Task<AdvertisementDao[]> Handle(
        GetAdvertisementsQuery query,
        CancellationToken ct = default
    )
    {
        GenericSpecificationBuilder<GetAdvertisementsQuery> builder = new(Sql, query);

        builder = builder
            .AppendFilterIf(
                "AND price_value >= @price_from",
                q => q.Filter?.PriceFilter is { PriceFrom: not null },
                q => q.Filter!.PriceFilter!.PriceFrom
            )
            .AppendFilterIf(
                "AND price_value <= @price_to",
                q => q.Filter?.PriceFilter is { PriceTo: not null },
                q => q.Filter!.PriceFilter!.PriceTo
            )
            .AppendFilterIf(
                "AND address ~ @address",
                q => q.Filter?.AddressFilter != null,
                q => q.Filter!.AddressFilter!.Address
            )
            .AppendTextSearchIf(
                q =>
                    q.Filter?.TextFilter != null
                    && !string.IsNullOrWhiteSpace(q.Filter.TextFilter.Text),
                q => q.Filter!.TextFilter!.Text,
                ["title", "description"]
            );

        AdvertisementsSpecificationBuilder<GetAdvertisementsQuery> adapted = builder.AdaptTo(
            AdvertisementsSpecificationBuilder<GetAdvertisementsQuery>.AdaptFactory
        );

        CharacteristicsFilter? ctxFilter = query.Filter?.CharacteristicsFilter;
        if (ctxFilter != null)
        {
            adapted = ctxFilter.Characteristics.Aggregate(
                adapted,
                (current, option) => current.AddCharacteristicsTextSearch(option)
            );
        }

        CustomSqlQuery sqlQuery = adapted
            .Create()
            .AddPagination(query.Pagination)
            .AddSorting(query.Sort);

        CommandDefinition command = new(sqlQuery.Sql, sqlQuery.Parameters, cancellationToken: ct);

        using IDbConnection connection = _factory.Create();
        IEnumerable<AdvertisementDao> data = await connection.QueryAsync<AdvertisementDao>(command);

        return data.ToArray();
    }
}
