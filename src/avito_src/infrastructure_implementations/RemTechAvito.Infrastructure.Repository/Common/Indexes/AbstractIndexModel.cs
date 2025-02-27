using MongoDB.Driver;

namespace RemTechAvito.Infrastructure.Repository.Common.Indexes;

internal abstract class AbstractIndexModel<T>
    where T : class
{
    private readonly List<CreateIndexModel<T>> _indexes = [];

    protected void Add(CreateIndexModel<T> index)
    {
        _indexes.Add(index);
    }

    public IReadOnlyCollection<CreateIndexModel<T>> Indexes => _indexes;

    public async Task UpdateIndexesAsync(MongoClient client, string collectionName, string dbName)
    {
        var db = client.GetDatabase(dbName);
        var collection = db.GetCollection<T>(collectionName);
        await collection.Indexes.DropAllAsync();
        await collection.Indexes.CreateManyAsync(_indexes);
    }
}
