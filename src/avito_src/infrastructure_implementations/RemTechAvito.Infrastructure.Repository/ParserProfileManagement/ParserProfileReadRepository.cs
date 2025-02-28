using MongoDB.Bson;
using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;
using RemTechAvito.Infrastructure.Contracts.Repository;
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

        var filter = Builders<ParserProfile>.Filter.Eq(
            profile => profile.Id,
            new ParserProfileId(guidId)
        );
        var db = client.GetDatabase(MongoDbOptions.Databse);
        var collection = db.GetCollection<ParserProfile>(ParserProfileMetadata.CollectionName);
        var query = collection.Find(filter);
        var profile = await query.FirstOrDefaultAsync(ct);

        if (profile == null)
        {
            var error = $"Not found with id: {id}";
            logger.Error(
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
        var db = client.GetDatabase(MongoDbOptions.Databse);
        var collection = db.GetCollection<ParserProfile>(ParserProfileMetadata.CollectionName);
        using var data = await collection.FindAsync(_ => true, cancellationToken: ct);
        var profiles = await data.ToListAsync(ct);
        List<ParserProfileResponse> response = [];
        foreach (var profile in profiles)
        {
            List<ParserProfileLinksResponse> links = [];
            foreach (var link in profile.Links)
            {
                IEnumerable<string> additions = [];
                var isWithAdditions = link.Unwrap<ParserProfileLinkWithAdditions>();
                if (isWithAdditions.IsSuccess)
                    additions = isWithAdditions.Value.Additions;
                links.Add(new ParserProfileLinksResponse(link.Name, link.Link, additions));
            }

            response.Add(
                new ParserProfileResponse(
                    profile.Id.Id,
                    profile.CreatedOn.Date,
                    profile.Name.Name,
                    profile.State.IsActive,
                    profile.State.Description,
                    links
                )
            );
        }

        return response;
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
        var db = client.GetDatabase(MongoDbOptions.Databse);
        var collection = db.GetCollection<ParserProfile>(ParserProfileMetadata.CollectionName);
        using var data = await collection.FindAsync(filter, cancellationToken: ct);
        var profiles = await data.ToListAsync(ct);
        List<ParserProfileResponse> response = [];
        foreach (var profile in profiles)
        {
            List<ParserProfileLinksResponse> links = [];
            foreach (var link in profile.Links)
            {
                IEnumerable<string> additions = [];
                var isWithAdditions = link.Unwrap<ParserProfileLinkWithAdditions>();
                if (isWithAdditions.IsSuccess)
                    additions = isWithAdditions.Value.Additions;
                links.Add(new ParserProfileLinksResponse(link.Name, link.Link, additions));
            }

            response.Add(
                new ParserProfileResponse(
                    profile.Id.Id,
                    profile.CreatedOn.Date,
                    profile.Name.Name,
                    profile.State.IsActive,
                    profile.State.Description,
                    links
                )
            );
        }

        return response;
    }
}
