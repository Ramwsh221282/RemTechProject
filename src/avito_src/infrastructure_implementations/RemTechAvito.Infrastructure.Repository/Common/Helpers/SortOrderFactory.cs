using MongoDB.Driver;

namespace RemTechAvito.Infrastructure.Repository.Common.Helpers;

internal static class SortOrderFactory
{
    public static SortDefinition<T>? Create<T>(string? order, string property)
    {
        if (string.IsNullOrWhiteSpace(order) || string.IsNullOrWhiteSpace(property))
            return null;
        return order switch
        {
            "ASC" => Builders<T>.Sort.Ascending(property),
            "DESC" => Builders<T>.Sort.Descending(property),
            _ => null,
        };
    }
}
