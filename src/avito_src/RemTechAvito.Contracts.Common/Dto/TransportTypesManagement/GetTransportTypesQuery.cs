namespace RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;

public sealed record GetTransportTypesQuery(
    GetTransportTypesQueryPagination? Pagination = null,
    GetTransportTypesImplementor? Implementor = null,
    GetTransportTypesQuerySortMode? Sort = null,
    GetTransportTypesTextSearch? TextSearch = null,
    params GetTransportTypeLinkCondition[] Links
);

// ASC, DESC, NONE
public sealed record GetTransportTypesQuerySortMode(string Mode);

public sealed record GetTransportTypesQueryPagination(int Page, int PageSize);

public sealed record GetTransportTypesTextSearch(string Text);

public sealed record GetTransportTypesImplementor(string Implementor);

public sealed record GetTransportTypeLinkCondition(string Link);
