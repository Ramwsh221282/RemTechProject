using MongoDB.Bson;
using MongoDB.Driver;

namespace RemTech.MongoDb.Service.Common.Dtos;

public record PriceFilterCriteria(long PriceValueA, long PriceValueB, string Criteria)
{
    public virtual FilterDefinition<T> Accept<T>(FilterDefinition<T> filter, string propertyName)
    {
        return filter;
    }
}

public sealed record PriceFilterMoreCriteria(long PriceValue)
    : PriceFilterCriteria(PriceValue, 0, "MORE")
{
    public override FilterDefinition<T> Accept<T>(FilterDefinition<T> filter, string propertyName)
    {
        BsonDocument expression = new(propertyName, new BsonDocument("$gte", PriceValueA));
        return Builders<T>.Filter.And([filter, expression]);
    }
}

public sealed record PriceFilterLessCriteria(long PriceValue)
    : PriceFilterCriteria(0, PriceValue, "LESS")
{
    public override FilterDefinition<T> Accept<T>(FilterDefinition<T> filter, string propertyName)
    {
        BsonDocument expression = new(propertyName, new BsonDocument("$lte", PriceValueB));
        return Builders<T>.Filter.And([filter, expression]);
    }
}

public sealed record PriceFilterRangeCriteria(long PriceValueA, long PriceValueB)
    : PriceFilterCriteria(PriceValueA, PriceValueB, "RANGE")
{
    public override FilterDefinition<T> Accept<T>(FilterDefinition<T> filter, string propertyName)
    {
        BsonDocument expression = new(
            propertyName,
            new BsonDocument() { { "$gte", PriceValueA }, { "$lte", PriceValueB } }
        );
        return Builders<T>.Filter.And([filter, expression]);
    }
}

public sealed record UnspecifiedFilterRangeCriteria() : PriceFilterCriteria(0, 0, "NONE");

public static class PriceFilterCriteriaExtensions
{
    public static PriceFilterCriteria Specify(this PriceFilterCriteria criteria) =>
        criteria.Criteria switch
        {
            "MORE" => new PriceFilterMoreCriteria(criteria.PriceValueA),
            "LESS" => new PriceFilterLessCriteria(criteria.PriceValueB),
            "RANGE" => new PriceFilterRangeCriteria(criteria.PriceValueA, criteria.PriceValueB),
            _ => new UnspecifiedFilterRangeCriteria(),
        };

    public static FilterDefinition<T> Accept<T>(
        this FilterDefinition<T> filter,
        string propertyName,
        PriceFilterCriteria? criteria
    ) => criteria == null ? filter : criteria.Accept(filter, propertyName);
}
