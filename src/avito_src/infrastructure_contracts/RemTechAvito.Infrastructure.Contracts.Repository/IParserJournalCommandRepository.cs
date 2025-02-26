using RemTechAvito.Core.ParserJournalManagement;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface IParserJournalCommandRepository
{
    Task<Result<Guid>> Add(ParserJournal journal, CancellationToken ct = default);
}
