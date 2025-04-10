using RemTech.Domain.ParserContext;
using RemTech.Shared.SDK.OptionPattern;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Application.ParserContext.Contracts;

public interface IParserWriteRepository
{
    Task<Result<Guid>> Add(Parser parser, CancellationToken ct = default);
    Task Save(Parser parser, CancellationToken ct = default);
    Task<Option<Parser>> GetByName(string name, CancellationToken ct = default);
}
