using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.ParserJournalsManagement;
using RemTechAvito.Core.ParserJournalManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository.ParserJournalsManagement;

internal sealed class ParserJournalQueryRepository(MongoClient client, ILogger logger)
    : IParserJournalQueryRepository
{
    public async Task<ParserJournalsResponse> Get(
        int page,
        int pageSize,
        CancellationToken ct = default
    )
    {
        if (page < 1 || pageSize < 1)
            return new ParserJournalsResponse([], 0);

        logger.Information(
            "{Repository} request for nameof {Response}. Status: started.",
            nameof(ParserJournalQueryRepository),
            nameof(ParserJournalDto)
        );

        var db = client.GetDatabase(ParserJournalMetadata.DbName);
        var collection = db.GetCollection<ParserJournal>(ParserJournalMetadata.CollectionName);
        var filter = Builders<ParserJournal>.Filter.Empty;
        var query = collection.Find(filter);
        var items = await query.Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync(ct);
        var response = items.Select(i => new ParserJournalDto()
        {
            Description = i.Description,
            Id = i.Id,
            Hours = i.Time.Hours,
            Minutes = i.Time.Minutes,
            Seconds = i.Time.Seconds,
            Source = i.Source,
            IsSuccess = i.IsSuccess,
            ItemsParsed = i.ItemsParsed,
            Error = i.ErrorMessage,
            CreatedOn = i.CreatedOn.Date,
        });

        logger.Information(
            "{Repository} request for nameof {Response}. Status: finished.",
            nameof(ParserJournalQueryRepository),
            nameof(ParserJournalDto)
        );

        return new ParserJournalsResponse(response, items.Count);
    }
}
