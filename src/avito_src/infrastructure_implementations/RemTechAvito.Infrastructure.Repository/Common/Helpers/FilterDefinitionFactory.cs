using MongoDB.Bson;
using MongoDB.Driver;
using RemTechCommon.Utils.Extensions;

namespace RemTechAvito.Infrastructure.Repository.Common.Helpers;

internal static class FilterDefinitionFactory
{
    public static FilterDefinition<T> CreateEquality<T>(string propertyName, string equalTo)
    {
        if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(equalTo))
            return Builders<T>.Filter.Empty;
        var bsonDocument = new BsonDocument(
            propertyName,
            new BsonDocument() { { "$eq", equalTo } }
        );
        return Builders<T>.Filter.And(bsonDocument);
    }

    public static FilterDefinition<T> CreateEquality<T>(
        this FilterDefinition<T> filter,
        string propertyName,
        string equalTo
    )
    {
        return Builders<T>.Filter.And([filter, CreateEquality<T>(propertyName, equalTo)]);
    }

    public static FilterDefinition<T> CreateTextSearch<T>(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Builders<T>.Filter.Empty;
        var cleaned = searchTerm.CleanString();
        return Builders<T>.Filter.Text(
            cleaned,
            new TextSearchOptions() { CaseSensitive = false, DiacriticSensitive = false }
        );
    }

    public static FilterDefinition<T> CreateTextSearch<T>(
        this FilterDefinition<T> filter,
        string? searchTerm
    )
    {
        return Builders<T>.Filter.And([filter, CreateTextSearch<T>(searchTerm)]);
    }

    public static FilterDefinition<T> CreateIn<T, U>(string propertyName, IEnumerable<U>? values)
    {
        if (string.IsNullOrWhiteSpace(propertyName) || values == null)
            return Builders<T>.Filter.Empty;
        var document = new BsonDocument(
            propertyName,
            new BsonDocument() { { "$in", new BsonArray(values) } }
        );
        return Builders<T>.Filter.And(document);
    }

    public static FilterDefinition<T> CreateIn<T, U>(
        this FilterDefinition<T> filter,
        string propertyName,
        IEnumerable<U>? values
    )
    {
        return Builders<T>.Filter.And([filter, CreateIn<T, U>(propertyName, values)]);
    }
}
