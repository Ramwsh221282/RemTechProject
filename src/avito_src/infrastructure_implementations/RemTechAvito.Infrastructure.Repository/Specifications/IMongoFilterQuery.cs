using MongoDB.Driver;

namespace RemTechAvito.Infrastructure.Repository.Specifications;

public interface IMongoFilterQuery<TDto, TEntity>
{
    void AddFilter(TDto dto, List<FilterDefinition<TEntity>> filters);
}
