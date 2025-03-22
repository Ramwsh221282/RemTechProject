using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Configurations.MongoDbConfiguration;
using RemTech.MongoDb.Service.Features.ParserManagement;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MongoDb.Service.Common.Models.ParsersManagement;

public sealed class ParserRepository
{
    private readonly MongoClient _client;

    public ParserRepository(MongoClient client) => _client = client;

    public async Task<Parser> Add(Parser parser)
    {
        IMongoDatabase db = _client.GetParsersDb();
        IMongoCollection<Parser> collection = db.GetParsersCol();
        await collection.InsertOneAsync(parser);
        return parser;
    }

    public async Task<Result> Contains(FilterDefinition<Parser> filter)
    {
        IMongoDatabase db = _client.GetParsersDb();
        IMongoCollection<Parser> collection = db.GetParsersCol();
        bool contains = await collection.Find(filter).AnyAsync();
        return contains ? Result.Success() : new Error("Parser configuration was not found.");
    }

    public async Task<Result<Parser>> QuerySingle(FilterDefinition<Parser> filter)
    {
        IMongoDatabase db = _client.GetParsersDb();
        IMongoCollection<Parser> collection = db.GetParsersCol();
        Parser? parser = await collection.Find(filter).FirstOrDefaultAsync();
        return parser == null ? new Error("Parser configuration was not found.") : parser;
    }

    public async Task<Result> Delete(FilterDefinition<Parser> filter)
    {
        IMongoDatabase db = _client.GetParsersDb();
        IMongoCollection<Parser> collection = db.GetParsersCol();
        DeleteResult result = await collection.DeleteOneAsync(filter);
        return result.DeletedCount switch
        {
            1 => Result.Success(),
            _ => new Error("Parser configuration was not deleted."),
        };
    }

    public async Task<Result> Update(FilterDefinition<Parser> filter, Parser parser)
    {
        IMongoDatabase db = _client.GetParsersDb();
        IMongoCollection<Parser> collection = db.GetParsersCol();
        ReplaceOneResult result = await collection.ReplaceOneAsync(filter, parser);
        return result.ModifiedCount == 0
            ? new Error("Parser configuration was not updated.")
            : Result.Success();
    }

    public async Task<Result<List<Parser>>> GetAll()
    {
        IMongoDatabase db = _client.GetParsersDb();
        IMongoCollection<Parser> collection = db.GetParsersCol();
        return await collection.Find(_ => true).ToListAsync();
    }
}
