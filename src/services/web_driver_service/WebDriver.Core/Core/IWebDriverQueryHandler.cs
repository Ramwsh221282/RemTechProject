using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Queries;

namespace WebDriver.Core.Core;

public interface IWebDriverQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> Execute(TQuery query);
}
