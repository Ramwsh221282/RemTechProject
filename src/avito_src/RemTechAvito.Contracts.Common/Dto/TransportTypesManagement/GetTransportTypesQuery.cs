namespace RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;

public sealed record GetTransportTypesQuery(
    GetTransportTypesQueryPagination Pagination,
    GetTransportTypesQuerySortMode? Sort = null,
    GetTransportTypesTextSearch? TextSearch = null
);

// ASC, DESC, NONE
public sealed record GetTransportTypesQuerySortMode(string Mode);

public sealed record GetTransportTypesQueryPagination(int Page, int PageSize);

public sealed record GetTransportTypesTextSearch(string Text);
