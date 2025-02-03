using RemTechCommon;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Contracts.Contracts;

public interface IWebDriverQuery<TResultType>
{
    Task<Result<TResultType>> Execute();
}
