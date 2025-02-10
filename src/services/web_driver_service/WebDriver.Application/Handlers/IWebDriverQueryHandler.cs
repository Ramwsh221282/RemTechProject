using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Application.Handlers;

public interface IWebDriverQueryHandler<in TQuery, TResult>
    where TQuery : IWebDriverQuery<TResult>
{
    Task<Result<TResult>> Execute(TQuery query);
}
