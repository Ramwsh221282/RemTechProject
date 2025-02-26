using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.ParserJournalsManagement;
using RemTechAvito.Core.ParserJournalManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository.ParserJournalsManagement;

internal sealed class ParserJournalQueryRepository(MongoClient client, ILogger logger)
    : IParserJournalQueryRepository
{
    public async Task<IEnumerable<ParserJournalResponse>> Get(
        int page,
        int pageSize,
        CancellationToken ct = default
    )
    {
        if (page < 1 || pageSize < 1)
            return [];

        logger.Information(
            "{Repository} request for nameof {Response}. Status: started.",
            nameof(ParserJournalQueryRepository),
            nameof(ParserJournalResponse)
        );

        var db = client.GetDatabase(ParserJournalMetadata.DbName);
        var collection = db.GetCollection<ParserJournal>(ParserJournalMetadata.CollectionName);
        var filter = Builders<ParserJournal>.Filter.Empty;
        var query = collection.Find(filter);
        var items = await query.Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync(ct);
        IEnumerable<ParserJournalResponse> response = items.Select(i => new ParserJournalResponse()
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
        });

        logger.Information(
            "{Repository} request for nameof {Response}. Status: finished.",
            nameof(ParserJournalQueryRepository),
            nameof(ParserJournalResponse)
        );

        return response;
    }
}
