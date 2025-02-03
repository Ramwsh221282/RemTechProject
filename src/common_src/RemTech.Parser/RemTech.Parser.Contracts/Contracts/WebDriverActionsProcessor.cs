using RemTechCommon;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Contracts.Contracts;

public abstract class WebDriverActionsProcessor
{
    public abstract Task<Result<TResultType>> Execute<TResultType>(
        IWebDriverQuery<TResultType> query
    );

    public abstract Task<Result> Execute(IWebDriverCommand command);
}
