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
            var error = "Invalid id";
            logger.Error(
                "{Repository} request for profile with id: {Id} Failed. Error: {Error}",
                nameof(ParserProfileReadRepository),
                id,
                error
            );
            return new Error(error);
        }

        var canConvert = Guid.TryParse(id, out var guidId);
        if (!canConvert)
        {
            var error = "Invalid id";
            logger.Error(
                "{Repository} request for profile with id: {Id} Failed. Error: {Error}",
                nameof(ParserProfileReadRepository),
                id,
                error
            );
            return new Error(error);
        }

        var uuid = new BsonBinaryData(guidId, GuidRepresentation.Standard);
        var document = new BsonDocument("_id", new BsonDocument() { { "$eq", uuid } });
        var filter = Builders<ParserProfile>.Filter.And(document);
        var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
        var collection = db.GetCollection<ParserProfile>(ParserProfileMetadata.CollectionName);
        using var cursor = await collection.FindAsync(filter, cancellationToken: ct);
        var profile = await cursor.FirstOrDefaultAsync(ct);

        if (profile == null)
        {
            var error = $"Not found with id: {id}";
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
        var profiles = await data.ToListAsync(ct);
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

    public async Task<IEnumerable<ParserProfileResponse>> GetActiveOnly(
        CancellationToken ct = default
    )
    {
        logger.Information(
            "{Repository} request for getting all active profiles.",
            nameof(ParserProfileReadRepository)
        );
        var document = new BsonDocument(
            "State.state_value",
            new BsonDocument() { { "$eq", true } }
        );
        var filter = Builders<ParserProfile>.Filter.And(document);
        var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
        var collection = db.GetCollection<ParserProfile>(ParserProfileMetadata.CollectionName);
        using var data = await collection.FindAsync(filter, cancellationToken: ct);
        var profiles = await data.ToListAsync(ct);
        return profiles.Select(p => new ParserProfileResponse(
            p.Id.Id,
            p.CreatedOn.Date,
            p.State.IsActive,
            p.State.Description,
            p.Links.Select(l => new ParserProfileLinksResponse(l.Id.Id, l.Body.Link, l.Body.Mark))
                .ToArray()
        ));
    }
}
