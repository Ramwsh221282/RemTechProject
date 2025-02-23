using RemTechAvito.Core.ParserProfileManagement;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface IParserProfileCommandRepository
{
    Task<Result<Guid>> Add(ParserProfile profile, CancellationToken ct = default);
    Task<Result<Guid>> Update(ParserProfile profile, CancellationToken ct = default);
    Task<Result<Guid>> Delete(string? id, CancellationToken ct = default);
}
