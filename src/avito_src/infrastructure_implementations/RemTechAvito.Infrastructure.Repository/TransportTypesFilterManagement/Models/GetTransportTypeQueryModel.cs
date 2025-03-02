using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportTypes;
using RemTechAvito.Infrastructure.Repository.Common.Helpers;

namespace RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement.Models;

internal sealed record GetTransportTypeQueryModel
{
    public FilterDefinition<TransportType> Filter { get; private init; } =
        FilterDefinition<TransportType>.Empty;

    public SortDefinition<TransportType>? Sort { get; private init; }
    public int Page { get; private init; }
    public int Size { get; private init; }

    public GetTransportTypeQueryModel WithSort(string? mode)
    {
        return this with { Sort = SortOrderFactory.Create<TransportType>(mode, "type_name") };
    }

    public GetTransportTypeQueryModel WithPagination(int page, int size)
    {
        return this with { Page = page, Size = size };
    }

    public GetTransportTypeQueryModel WithTextSearch(string? searchTerm)
    {
        return this with { Filter = Filter.AddTextSearch(searchTerm) };
    }

    public GetTransportTypeQueryModel WithLinksIn(IEnumerable<string> links)
    {
        return this with { Filter = Filter.AddIn("type_link", links) };
    }

    public GetTransportTypeQueryModel WithTypeImplementor(string? implementor)
    {
        return this with { Filter = Filter.AddEquality("type_implementor", implementor) };
    }
}

internal static class GetTransportTypeQueryModelExtensions
{
    public static GetTransportTypeQueryModel Create(this GetTransportTypesQuery query)
    {
        var page = query.Pagination?.Page;
        var size = query.Pagination?.PageSize;
        var sortMode = query.Sort?.Mode;
        var searchTerm = query.TextSearch?.Text;
        var implementor = query.Implementor?.Implementor;
        var links = query.Links.Select(l => l.Link);
        return new GetTransportTypeQueryModel()
            .WithPagination(page ?? 0, size ?? 0)
            .WithSort(sortMode)
            .WithTextSearch(searchTerm)
            .WithTypeImplementor(implementor)
            .WithLinksIn(links);
    }
}
