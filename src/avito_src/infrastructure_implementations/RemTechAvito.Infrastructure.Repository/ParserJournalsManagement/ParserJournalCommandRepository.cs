using MongoDB.Driver;
using RemTechAvito.Core.ParserJournalManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository.ParserJournalsManagement;

internal sealed class ParserJournalCommandRepository(MongoClient client, ILogger logger)
    : IParserJournalCommandRepository
{
    public async Task<Result<Guid>> Add(ParserJournal journal, CancellationToken ct = default)
    {
        try
        {
            var db = client.GetDatabase(ParserJournalMetadata.DbName);
            var collection = db.GetCollection<ParserJournal>(ParserJournalMetadata.CollectionName);
            await collection.InsertOneAsync(journal, cancellationToken: ct);
            logger.Information(
                "{Repository} added Journal {id}",
                nameof(ParserJournalCommandRepository),
                journal.Id
            );
            return journal.Id;
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Repository} cannot add Journal {Id}. Exception: {Ex}",
                nameof(ParserJournalCommandRepository),
                journal.Id,
                ex.Message
            );
            return new Error($"Cannot add journal with id: {journal.Id}. Internal server error");
        }
    }
}
