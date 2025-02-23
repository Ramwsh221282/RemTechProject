using RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface IParserProfileReadRepository
{
    Task<Result<ParserProfile>> GetById(string? id, CancellationToken ct = default);
    Task<IReadOnlyCollection<ParserProfileResponse>> Get(CancellationToken ct = default);
}
