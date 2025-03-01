using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportTypes;
using RemTechAvito.Infrastructure.Repository.Common.Helpers;

namespace RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement.Models;

internal sealed record GetTransportTypeQueryModel(
    FilterDefinition<TransportType> Filter,
    SortDefinition<TransportType>? Sort,
    int Page = 0,
    int Size = 0
);

internal sealed class GetTransportTypeQueryModelExtensions
{
    public static GetTransportTypeQueryModel Create(GetTransportTypesQuery query)
    {
        return new GetTransportTypeQueryModel(
            FilterDefinitionFactory
                .CreateEquality<TransportType>(
                    "type_implementor",
                    query.Implementor?.Implementor ?? ""
                )
                .CreateTextSearch(query.TextSearch?.Text)
                .CreateIn("type_link", query.Links.Select(l => l.Link)),
            SortOrderFactory.Create<TransportType>(query.Sort?.Mode, "type_name"),
            query.Pagination?.Page ?? 0,
            query.Pagination?.PageSize ?? 0
        );
    }
}
