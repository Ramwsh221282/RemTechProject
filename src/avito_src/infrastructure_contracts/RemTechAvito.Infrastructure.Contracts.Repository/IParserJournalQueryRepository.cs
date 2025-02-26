using RemTechAvito.Contracts.Common.Dto.ParserJournalsManagement;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface IParserJournalQueryRepository
{
    Task<ParserJournalsResponse> Get(int page, int pageSize, CancellationToken ct = default);
}
