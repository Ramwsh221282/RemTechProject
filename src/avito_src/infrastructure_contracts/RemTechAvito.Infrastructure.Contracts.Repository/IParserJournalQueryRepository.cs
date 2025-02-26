using RemTechAvito.Contracts.Common.Dto.ParserJournalsManagement;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface IParserJournalQueryRepository
{
    Task<IEnumerable<ParserJournalResponse>> Get(
        int page,
        int pageSize,
        CancellationToken ct = default
    );
}
