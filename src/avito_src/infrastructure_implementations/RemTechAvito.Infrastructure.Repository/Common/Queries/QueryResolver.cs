using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.Common.Queries;

public abstract class QueryResolver<TDto, TEntity>
{
    protected readonly List<FilterDefinition<TEntity>> _filters = [];
    protected readonly IServiceScopeFactory _factory;

    public QueryResolver(IServiceScopeFactory factory)
    {
        _factory = factory;
    }

    public FilterDefinition<TEntity> Resolve(TDto dto)
    {
        using IServiceScope scope = _factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        IEnumerable<IMongoFilterQuery<TDto, TEntity>> queries = provider.GetServices<
            IMongoFilterQuery<TDto, TEntity>
        >();
        foreach (var query in queries)
            query.AddFilter(dto, _filters);

        var builder = Builders<TEntity>.Filter;
        var result = builder.And(_filters);
        _filters.Clear();
        return result;
    }
}
