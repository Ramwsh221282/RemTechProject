using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.Common.Queries;

public abstract class QueryResolver<TDto, TEntity>
{
    private readonly List<FilterDefinition<TEntity>> _filters = [];
    private readonly IServiceScopeFactory _factory;

    protected QueryResolver(IServiceScopeFactory factory) => _factory = factory;

    public FilterDefinition<TEntity> Resolve(TDto? dto)
    {
        if (dto == null)
            return Builders<TEntity>.Filter.Empty;

        using IServiceScope scope = _factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        IEnumerable<IMongoFilterQuery<TDto, TEntity>> queries = provider.GetServices<
            IMongoFilterQuery<TDto, TEntity>
        >();

        foreach (var query in queries)
            query.AddFilter(dto, _filters);

        if (_filters.Count == 0)
            return Builders<TEntity>.Filter.Empty;

        var builder = Builders<TEntity>.Filter;
        var result = builder.And(_filters);
        _filters.Clear();
        return result;
    }
}
