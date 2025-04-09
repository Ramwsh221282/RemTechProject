using RemTech.Shared.SDK.CqrsPattern.Queries;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisements;

public sealed record GetAdvertisementsQueryDto(FilterOptions? Filter);

public sealed record GetAdvertisementsQuery(
    PaginationOption Pagination,
    FilterOptions? Filter,
    SortMode? Sort
) : IQuery;

public sealed record PaginationOption(int Page, int PageSize);

public sealed record FilterOptions(
    PriceFilter? PriceFilter,
    AddressFilter? AddressFilter,
    TextFilter? TextFilter,
    CharacteristicsFilter? CharacteristicsFilter
);

public sealed record PriceFilter(long? PriceFrom, long? PriceTo);

public sealed record AddressFilter(string Address);

public sealed record TextFilter(string Text);

public sealed record SortMode(string Mode);

public sealed record CharacteristicsFilter(CharacteristicOption[] Characteristics);

public sealed record CharacteristicOption(string Name, string Value);
