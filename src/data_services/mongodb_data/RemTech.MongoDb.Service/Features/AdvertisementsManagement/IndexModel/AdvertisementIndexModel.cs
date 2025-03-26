using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Attributes;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;
using RemTech.MongoDb.Service.Configurations.MongoDbConfiguration;

namespace RemTech.MongoDb.Service.Features.AdvertisementsManagement.IndexModel;

[IndexModel]
public static class AdvertisementIndexModel
{
    [IndexModelMethod]
    public static void RegisterIndexes(this MongoClient client)
    {
        IMongoCollection<Advertisement> collection = client
            .GetAdvertisementsDb()
            .GetAdvertisementsCol();
        List<CreateIndexModel<Advertisement>> indexes = [];
        indexes.Add(new(Builders<Advertisement>.IndexKeys.Ascending("AdvertisementId")));
        indexes.Add(new(Builders<Advertisement>.IndexKeys.Ascending("Price")));
        indexes.Add(new(Builders<Advertisement>.IndexKeys.Ascending("Title")));
        indexes.Add(new(Builders<Advertisement>.IndexKeys.Ascending("Description")));
        indexes.Add(new(Builders<Advertisement>.IndexKeys.Ascending("PriceExtra")));
        indexes.Add(new(Builders<Advertisement>.IndexKeys.Ascending("Address")));
        indexes.Add(new(Builders<Advertisement>.IndexKeys.Ascending("Characteristics.name")));
        indexes.Add(new(Builders<Advertisement>.IndexKeys.Ascending("Characteristics.value")));
        indexes.Add(
            new(
                Builders<Advertisement>
                    .IndexKeys.Ascending("Characteristics.name")
                    .Ascending("Characteristics.value")
            )
        );
        collection.Indexes.DropAll();
        collection.Indexes.CreateMany(indexes);
    }
}
