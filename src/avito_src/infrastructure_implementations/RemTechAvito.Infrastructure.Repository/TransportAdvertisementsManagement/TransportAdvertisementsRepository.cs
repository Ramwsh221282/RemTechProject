using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;

public sealed class TransportAdvertisementsRepository : ITransportAdvertisementsRepository
{
    private readonly ILogger _logger;
    private readonly MongoClient _client;
    public const string DbName = "Transport_Advertisements_Db";
    private const string CollectionName = "Transport_Advertisements_Collection";

    public TransportAdvertisementsRepository(ILogger logger, MongoClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<Guid> Add(
        TransportAdvertisement advertisement,
        CancellationToken ct = default
    )
    {
        try
        {
            var db = _client.GetDatabase(DbName);
            await db.CreateCollectionAsync(CollectionName, cancellationToken: ct);
            var collection = db.GetCollection<TransportAdvertisement>(CollectionName);
            await collection.InsertOneAsync(advertisement, cancellationToken: ct);

            return advertisement.EntityId.Id;
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "{ClassName} exception on insert: {Ex}",
                nameof(TransportAdvertisementsRepository),
                ex.Message
            );
            return Guid.Empty;
        }
    }

    public static void RegisterSerializers()
    {
        BsonSerializer.RegisterSerializer(new AddressSerializer());
        BsonSerializer.RegisterSerializer(new AdvertisementIDSerializer());
        BsonSerializer.RegisterSerializer(new CharacteristicsSerializer());
        BsonSerializer.RegisterSerializer(new DescriptionSerializer());
        BsonSerializer.RegisterSerializer(new EntityIDSerializer());
        BsonSerializer.RegisterSerializer(new OwnerInformationSerializer());
        BsonSerializer.RegisterSerializer(new PhotoAttachmentsSerializer());
        BsonSerializer.RegisterSerializer(new PriceSerializer());
        BsonSerializer.RegisterSerializer(new TitleSerializer());
        BsonSerializer.RegisterSerializer(new AdvertisementUrlSerializer());
    }
}
