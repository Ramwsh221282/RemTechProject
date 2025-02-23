using MongoDB.Bson;
using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Mappers;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement;

internal sealed class ParserProfileReadRepository(MongoClient client, ILogger logger)
    : IParserProfileReadRepository
{
    public async Task<Result<ParserProfile>> GetById(string? id, CancellationToken ct = default)
    {
        logger.Information(
            "{Repository} request for profile with id: {Id}",
            nameof(ParserProfileReadRepository),
            id
        );

        if (string.IsNullOrWhiteSpace(id))
        {
            string error = "Invalid id";
            logger.Error(
                "{Repository} request for profile with id: {Id} Failed. Error: {Error}",
                nameof(ParserProfileReadRepository),
                id,
                error
            );
            return new Error(error);
        }

        bool canConvert = Guid.TryParse(id, out Guid guidId);
        if (!canConvert)
        {
            string error = "Invalid id";
            logger.Error(
                "{Repository} request for profile with id: {Id} Failed. Error: {Error}",
                nameof(ParserProfileReadRepository),
                id,
                error
            );
            return new Error(error);
        }

        var uuid = new BsonBinaryData(guidId, GuidRepresentation.Standard);
        BsonDocument document = new BsonDocument("_id", new BsonDocument() { { "$eq", uuid } });
        FilterDefinition<ParserProfile> filter = Builders<ParserProfile>.Filter.And(document);
        var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
        var collection = db.GetCollection<ParserProfile>(ParserProfileMetadata.CollectionName);
        using var cursor = await collection.FindAsync(filter, cancellationToken: ct);
        ParserProfile? profile = await cursor.FirstOrDefaultAsync(cancellationToken: ct);

        if (profile == null)
        {
            string error = $"Not found with id: {id}";
            logger.Warning(
                "{Repository} request for profile with id: {Id}. Not Found.",
                nameof(ParserProfileReadRepository),
                id
            );
            return new Error(error);
        }

        logger.Information(
            "{Repository} received profile {Id}",
            nameof(ParserProfileCommandRepository),
            id
        );

        return profile;
    }

    public async Task<IReadOnlyCollection<ParserProfileResponse>> Get(
        CancellationToken ct = default
    )
    {
        logger.Information(
            "{Repository} request for getting all profiles.",
            nameof(ParserProfileReadRepository)
        );
        var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
        var collection = db.GetCollection<ParserProfile>(ParserProfileMetadata.CollectionName);
        using var data = await collection.FindAsync(_ => true, cancellationToken: ct);
        List<ParserProfile> profiles = await data.ToListAsync(cancellationToken: ct);
        return profiles
            .Select(p => new ParserProfileResponse(
                p.Id.Id,
                p.CreatedOn.Date,
                p.State.IsActive,
                p.State.Description,
                p.Links.Select(l => new ParserProfileLinksResponse(
                        l.Id.Id,
                        l.Body.Link,
                        l.Body.Mark
                    ))
                    .ToArray()
            ))
            .ToList();
    }
}
