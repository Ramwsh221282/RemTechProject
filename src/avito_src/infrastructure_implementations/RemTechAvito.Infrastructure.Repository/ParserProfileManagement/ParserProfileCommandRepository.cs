using MongoDB.Bson;
using MongoDB.Driver;
using RemTechAvito.Core.ParserProfileManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement;

internal sealed class ParserProfileCommandRepository(ILogger logger, MongoClient client)
    : IParserProfileCommandRepository
{
    public async Task<Result<Guid>> Add(ParserProfile profile, CancellationToken ct = default)
    {
        try
        {
            var db = client.GetDatabase(MongoDbOptions.Databse);
            await db.CreateCollectionAsync(
                ParserProfileMetadata.CollectionName,
                cancellationToken: ct
            );
            var collection = db.GetCollection<ParserProfile>(ParserProfileMetadata.CollectionName);
            await collection.InsertOneAsync(profile, cancellationToken: ct);
            logger.Information(
                "{Repository} added profile {Id}",
                nameof(ParserProfileCommandRepository),
                profile.Id.Id
            );
            return profile.Id.Id;
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Repository} profile was not added. Error: {Ex}",
                nameof(ParserProfileCommandRepository),
                ex.Message
            );
            return new Error("Cannot add new profile. Internal server error.");
        }
    }

    public async Task<Result<Guid>> Update(ParserProfile profile, CancellationToken ct = default)
    {
        try
        {
            var db = client.GetDatabase(MongoDbOptions.Databse);
            await db.CreateCollectionAsync(
                ParserProfileMetadata.CollectionName,
                cancellationToken: ct
            );
            var collection = db.GetCollection<ParserProfile>(ParserProfileMetadata.CollectionName);
            var uuid = new BsonBinaryData(profile.Id.Id, GuidRepresentation.Standard);
            var document = new BsonDocument("_id", new BsonDocument() { { "$eq", uuid } });
            var definition = Builders<ParserProfile>.Filter.And(document);
            var updateResult = await collection.ReplaceOneAsync(
                definition,
                profile,
                cancellationToken: ct
            );

            if (!updateResult.IsAcknowledged)
            {
                var error =
                    $"Cannot update profile with id: {profile.Id.Id}. Internal server error.";
                logger.Error("{Repository} {Error}", nameof(ParserProfileCommandRepository), error);
                return new Error(error);
            }

            logger.Information(
                "{Repository} updated profile with id {Id}",
                nameof(ParserProfileCommandRepository),
                profile.Id.Id
            );
            return profile.Id.Id;
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Repository} profile was not updated. Error: {Ex}",
                nameof(ParserProfileCommandRepository),
                ex.Message
            );
            return new Error("Cannot update profile. Internal server error.");
        }
    }

    public async Task<Result<Guid>> Delete(string? id, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                var error = "Id is invalid";
                logger.Error(
                    "{Repository} cannot delete profile. Id is invalid",
                    nameof(ParserProfileCommandRepository)
                );
                return new Error(error);
            }

            var canParse = Guid.TryParse(id, out var parsedGuid);
            if (!canParse)
            {
                var error = "Id is invalid";
                logger.Error(
                    "{Repository} cannot delete profile. Id is invalid",
                    nameof(ParserProfileCommandRepository)
                );
                return new Error(error);
            }

            var uuid = new BsonBinaryData(parsedGuid, GuidRepresentation.Standard);
            var db = client.GetDatabase(MongoDbOptions.Databse);
            await db.CreateCollectionAsync(
                ParserProfileMetadata.CollectionName,
                cancellationToken: ct
            );
            var collection = db.GetCollection<ParserProfile>(ParserProfileMetadata.CollectionName);
            var document = new BsonDocument("_id", new BsonDocument() { { "$eq", uuid } });
            var definition = Builders<ParserProfile>.Filter.And(document);

            var deleteResult = await collection.DeleteOneAsync(definition, ct);
            if (!deleteResult.IsAcknowledged || deleteResult.DeletedCount == 0)
            {
                var error = $"Cannot delete profile. Not found with id {parsedGuid}.";
                logger.Error(
                    "{Repository} error: {Error}",
                    nameof(ParserProfileCommandRepository),
                    error
                );
                return new Error(error);
            }

            logger.Information(
                "{Repository} deleted profile with id {id}",
                nameof(ParserProfileCommandRepository),
                parsedGuid
            );
            return parsedGuid;
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Repository} profile was not deleted. Error: {Ex}",
                nameof(ParserProfileCommandRepository),
                ex.Message
            );
            return new Error("Cannot delete profile. Internal server error.");
        }
    }
}
