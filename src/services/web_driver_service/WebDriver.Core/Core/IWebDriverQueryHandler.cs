using RemTech.WebDriver.Plugin.Queries;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.WebDriver.Plugin.Core;

internal interface IWebDriverQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> Execute(TQuery query);
}
