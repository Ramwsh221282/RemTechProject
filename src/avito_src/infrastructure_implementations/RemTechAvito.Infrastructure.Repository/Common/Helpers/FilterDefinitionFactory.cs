using MongoDB.Bson;
using MongoDB.Driver;
using RemTechAvito.Core.AdvertisementManagement.TransportTypes;
using RemTechCommon.Utils.Extensions;

namespace RemTechAvito.Infrastructure.Repository.Common.Helpers;

internal static class FilterDefinitionFactory
{
    public static FilterDefinition<T> AddEquality<T>(
        this FilterDefinition<T> filter,
        string? propertyName,
        string? equalTo
    )
    {
        if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(equalTo))
            return filter;
        var bsonDocument = new BsonDocument(
            propertyName,
            new BsonDocument() { { "$eq", equalTo } }
        );
        return Builders<T>.Filter.And([filter, bsonDocument]);
    }

    public static FilterDefinition<T> AddTextSearch<T>(
        this FilterDefinition<T> filter,
        string? searchTerm
    )
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return filter;
        var cleaned = searchTerm.CleanString();
        return Builders<T>.Filter.And(
            [
                filter,
                Builders<T>.Filter.Text(
                    cleaned,
                    new TextSearchOptions() { CaseSensitive = false, DiacriticSensitive = false }
                ),
            ]
        );
    }

    public static FilterDefinition<T> AddIn<T, U>(
        this FilterDefinition<T> filter,
        string? propertyName,
        IEnumerable<U>? values
    )
    {
        if (string.IsNullOrWhiteSpace(propertyName) || values == null || !values.Any())
            return filter;
        var document = new BsonDocument(
            propertyName,
            new BsonDocument() { { "$in", new BsonArray(values) } }
        );
        return Builders<T>.Filter.And([filter, document]);
    }
}
