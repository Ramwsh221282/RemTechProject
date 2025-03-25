using MongoDB.Bson;
using MongoDB.Driver;
using RemTech.MongoDb.Service.Configurations.MongoDbConfiguration;
using RemTech.MongoDb.Service.Features.CharacteristicsManagement.Models;

namespace RemTech.MongoDb.Service.Features.CharacteristicsManagement.Data;

public sealed class CharacteristicsRepository(MongoClient client)
{
    private readonly MongoClient _client = client;

    public async Task<bool> Contains(Characteristic characteristic)
    {
        IMongoCollection<Characteristic> collection = GetCollection();
        BsonDocument filter = new("Name", new BsonDocument("$eq", characteristic.Name));
        return await collection.Find(filter).AnyAsync();
    }

    public async Task Save(Characteristic characteristic)
    {
        IMongoCollection<Characteristic> collection = GetCollection();
        await collection.InsertOneAsync(characteristic);
    }

    public async Task<Characteristic[]> Get()
    {
        IMongoCollection<Characteristic> collection = GetCollection();
        List<Characteristic> items = await collection.Find(_ => true).ToListAsync();
        return items.ToArray();
    }

    private IMongoCollection<Characteristic> GetCollection() =>
        _client.GetAdvertisementsDb().GetCharacteristicsCol();
}
