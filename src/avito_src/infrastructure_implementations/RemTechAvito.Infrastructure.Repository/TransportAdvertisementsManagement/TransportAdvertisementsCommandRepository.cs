using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;

internal sealed class TransportAdvertisementsCommandRepository
    : ITransportAdvertisementsCommandRepository
{
    private readonly ILogger _logger;
    private readonly MongoClient _client;

    public TransportAdvertisementsCommandRepository(ILogger logger, MongoClient client)
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
            var db = _client.GetDatabase(TransportAdvertisementsRepository.DbName);
            await db.CreateCollectionAsync(
                TransportAdvertisementsRepository.CollectionName,
                cancellationToken: ct
            );
            var collection = db.GetCollection<TransportAdvertisement>(
                TransportAdvertisementsRepository.CollectionName
            );

            await collection.InsertOneAsync(advertisement, cancellationToken: ct);
            _logger.Information(
                "{Repository} inserted advertisement ({Id})",
                nameof(TransportAdvertisementsCommandRepository),
                advertisement.EntityId.Id
            );
            return advertisement.EntityId.Id;
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "{ClassName} exception on insert: {Ex}",
                nameof(TransportAdvertisementsCommandRepository),
                ex.Message
            );
            return Guid.Empty;
        }
    }
}
