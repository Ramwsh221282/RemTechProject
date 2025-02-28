using MongoDB.Driver;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;

internal sealed class TransportAdvertisementsCommandRepository(ILogger logger, MongoClient client)
    : ITransportAdvertisementsCommandRepository
{
    public async Task<Guid> Add(
        TransportAdvertisement advertisement,
        CancellationToken ct = default
    )
    {
        try
        {
            var db = client.GetDatabase(MongoDbOptions.Databse);
            await db.CreateCollectionAsync(
                TransportAdvertisementsMetadata.Collection,
                cancellationToken: ct
            );
            var collection = db.GetCollection<TransportAdvertisement>(
                TransportAdvertisementsMetadata.Collection
            );

            await collection.InsertOneAsync(advertisement, cancellationToken: ct);
            logger.Information(
                "{Repository} inserted advertisement ({Id})",
                nameof(TransportAdvertisementsCommandRepository),
                advertisement.TransportAdvertisementId.Id
            );
            return advertisement.TransportAdvertisementId.Id;
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{ClassName} exception on insert: {Ex}",
                nameof(TransportAdvertisementsCommandRepository),
                ex.Message
            );
            return Guid.Empty;
        }
    }
}
