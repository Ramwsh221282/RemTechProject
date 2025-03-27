using MongoDB.Bson;
using MongoDB.Driver;

namespace RemTech.MongoDb.Service.Common.Abstractions.FilterDefinitionUtils;

public static class FilterDefinitionExtensions
{
    public static FilterDefinition<TEntity> ApplyAnd<TEntity>(this FilterDefinition<TEntity> filterA,
        FilterDefinition<TEntity> filterB)
    {
        return Builders<TEntity>.Filter.And(filterA, filterB);
    }

    public static FilterDefinition<TEntity> ApplyAnd<TEntity>(this FilterDefinition<TEntity> filterA,
        BsonDocument filterB)
    {
        return Builders<TEntity>.Filter.And(filterA, filterB);
    }
}