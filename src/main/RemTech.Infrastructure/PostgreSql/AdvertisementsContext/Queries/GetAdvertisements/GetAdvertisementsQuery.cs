using RemTech.Infrastructure.PostgreSql.Shared;
using RemTech.Shared.SDK.CqrsPattern.Queries;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisements;

public sealed record GetAdvertisementsQueryDto(FilterOptions? Filter);

public sealed record GetAdvertisementsQuery(
    PaginationOptions Pagination,
    FilterOptions? Filter,
    SortOptions? Sort
) : IQuery;

public sealed record FilterOptions(
    PriceFilter? PriceFilter,
    AddressFilter? AddressFilter,
    TextFilter? TextFilter,
    CharacteristicsFilter? CharacteristicsFilter
);

public sealed record PriceFilter(long? PriceFrom, long? PriceTo);

public sealed record AddressFilter(string Address);

public sealed record CharacteristicsFilter(CharacteristicOption[] Characteristics);

public sealed record CharacteristicOption(string Name, string Value);
