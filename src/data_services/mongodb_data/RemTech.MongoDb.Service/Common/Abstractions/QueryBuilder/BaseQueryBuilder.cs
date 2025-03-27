using MongoDB.Bson;
using MongoDB.Driver;

namespace RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;

public abstract class BaseQueryBuilder<TModel>
{
    private readonly List<FilterDefinition<TModel>> _filters = [];
    public FilterDefinition<TModel> Empty => Builders<TModel>.Filter.Empty;

    public void With(BsonDocument next) => _filters.Add(Builders<TModel>.Filter.And(next));

    public FilterDefinition<TModel> Combined =>
        _filters.Count == 0 ? Builders<TModel>.Filter.Empty : Builders<TModel>.Filter.And(_filters);
}
