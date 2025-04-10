using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisements;
using RemTech.Infrastructure.PostgreSql.Shared;

namespace Infrastructure.Tests.QueryBuilderTests.Models;

public sealed class QueryBuilderTests
{
    [Fact]
    public void Invoke()
    {
        PriceFilter? priceFilter = new(1000, 2000);
        AddressFilter? addressFilter = null;
        CharacteristicsFilter? characteristicsFilter = null;
        TextFilter? textFilter = null;
        FilterOptions filterOptions = new(
            priceFilter,
            addressFilter,
            textFilter,
            characteristicsFilter
        );
        GetAdvertisementsQueryDto dto = new(filterOptions);

        bool isNull = dto.Filter?.PriceFilter?.PriceFrom != null;

        CustomGenericQueryBuilder builder = new CustomGenericQueryBuilder();
        builder = builder
            .Select("*")
            .From("advertisements")
            .WhereIf(
                dto,
                d => d.Filter?.PriceFilter?.PriceFrom != null,
                d => d.Filter!.PriceFilter!.PriceFrom!.Value,
                "price_value >= @price_from"
            )
            .WhereIf(
                dto,
                d => d.Filter?.PriceFilter?.PriceTo != null,
                d => d.Filter!.PriceFilter!.PriceTo!.Value,
                "price_value <= @price_to"
            );

        string sql = builder.Sql;
        int bpoint = 0;
    }
}
