using RemTech.Parser.Contracts.Contracts;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Implementation.Core;

public interface IWebDriverQueryHandler<in TQuery, TResult>
    where TQuery : IWebDriverQuery<TResult>
{
    Task<Result<TResult>> Execute(TQuery query);
}
