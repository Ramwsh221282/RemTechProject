using MongoDB.Bson;

namespace RemTech.MongoDb.Service.Common.Abstractions.BsonRegularExpressionUtils;

public static class BsonRegularExpressionExtensions
{
    public static BsonRegularExpression CreateBsonRegularExpressionFromString(this string input, bool ignoreCase = true)
    {
        return ignoreCase switch
        {
            true => new BsonRegularExpression($".*{input}.*", "i"),
            false => new BsonRegularExpression($".*{input}.*")
        };
    }

    public static BsonDocument CreateBsonDocumentWithRegularExpressionFilter(this string input, string propertyName,
        bool ignoreCase = true)
    {
        BsonRegularExpression expression = input.CreateBsonRegularExpressionFromString(ignoreCase);
        BsonDocument filter = new BsonDocument("$regex", expression);
        BsonDocument document = new BsonDocument(propertyName, filter);
        return document;
    }
}