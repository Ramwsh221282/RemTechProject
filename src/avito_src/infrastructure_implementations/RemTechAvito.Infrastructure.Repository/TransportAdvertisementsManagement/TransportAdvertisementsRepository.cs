using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Common.Serializers;
using RemTechAvito.Infrastructure.Repository.CustomerStatesFilterManagement;
using RemTechAvito.Infrastructure.Repository.CustomerStatesFilterManagement.Serializers;
using RemTechAvito.Infrastructure.Repository.CustomerTypesFilterManagement;
using RemTechAvito.Infrastructure.Repository.CustomerTypesFilterManagement.Serializers;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;
using RemTechAvito.Infrastructure.Repository.TransportStatesFilterManagement;
using RemTechAvito.Infrastructure.Repository.TransportStatesFilterManagement.Serializers;
using RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement;
using RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement.Serializers;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;

internal static class TransportAdvertisementsRepository
{
    public const string DbName = "Avito_Db";
    public const string CollectionName = "Transport_Advertisements";

    public static void RegisterSerializers()
    {
        BsonSerializer.RegisterSerializer(new AddressSerializer());
        BsonSerializer.RegisterSerializer(new AdvertisementIDSerializer());
        BsonSerializer.RegisterSerializer(new CharacteristicsSerializer());
        BsonSerializer.RegisterSerializer(new DescriptionSerializer());
        BsonSerializer.RegisterSerializer(new EntityIdSerializer());
        BsonSerializer.RegisterSerializer(new OwnerInformationSerializer());
        BsonSerializer.RegisterSerializer(new PhotoAttachmentsSerializer());
        BsonSerializer.RegisterSerializer(new PriceSerializer());
        BsonSerializer.RegisterSerializer(new TitleSerializer());
        BsonSerializer.RegisterSerializer(new AdvertisementUrlSerializer());
        BsonSerializer.RegisterSerializer(new CustomerStateSerializer());
        BsonSerializer.RegisterSerializer(new CustomerTypeSerializer());
        BsonSerializer.RegisterSerializer(new TransportStateSerializer());
        BsonSerializer.RegisterSerializer(new TransportTypeSerializer());
    }

    public static void RegisterBsonClassMap()
    {
        BsonClassMap.RegisterClassMap(new TransportAdvertisementClassMap());
        BsonClassMap.RegisterClassMap(new CustomerStatesBsonClassMap());
        BsonClassMap.RegisterClassMap(new CustomerTypesBsonClassMap());
        BsonClassMap.RegisterClassMap(new TransportStatesBsonClassMap());
        BsonClassMap.RegisterClassMap(new TransportTypesBsonClassMap());
    }

    public static async Task RegisterIndexes(MongoClient client, CancellationToken ct = default)
    {
        try
        {
            var textIndexModel = new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>
                    .IndexKeys.Text("Address")
                    .Text("Description")
                    .Text("Title")
                    .Text("CharacteristicsSearch.attribute_name")
                    .Text("CharacteristicsSearch.attribute_value")
                    .Text("OwnerInformation.owner_description"),
                new CreateIndexOptions()
                {
                    Name = "Address_Description_Title_Characteristics_OwnerInformation_Text",
                }
            );

            var advertisementIdIndexModel = new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>.IndexKeys.Ascending("AdvertisementId"),
                new CreateIndexOptions() { Name = "AdvertisementId_Index", Unique = true }
            );

            var priceIndexModel = new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>
                    .IndexKeys.Ascending("Price.price_value")
                    .Ascending("Price.price_currency")
                    .Ascending("Price.price_extra"),
                new CreateIndexOptions { Name = "Price_Value_Currency_Extra" }
            );

            var dateIndexModel = new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>.IndexKeys.Ascending("CreatedOn"),
                new CreateIndexOptions { Name = "CreatedOn_Index" }
            );

            var characteristicsIndexModel = new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>
                    .IndexKeys.Ascending("CharacteristicsSearch.attribute_name")
                    .Ascending("CharacteristicsSearch.attribute_value"),
                new CreateIndexOptions { Name = "Characteristics_Name_Value" }
            );

            var ownerIndexModel = new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>
                    .IndexKeys.Ascending("OwnerInformation.owner_description")
                    .Ascending("OwnerInformation.owner_status")
                    .Ascending("OwnerInformation.owner_contacts"),
                new CreateIndexOptions { Name = "OwnerInformation_Description_Status_Contacts" }
            );

            var db = client.GetDatabase(DbName);
            var collection = db.GetCollection<TransportAdvertisement>(CollectionName);

            await collection.Indexes.CreateManyAsync(
                [
                    textIndexModel,
                    priceIndexModel,
                    dateIndexModel,
                    ownerIndexModel,
                    characteristicsIndexModel,
                    advertisementIdIndexModel,
                ],
                ct
            );
        }
        catch { }
    }
}
