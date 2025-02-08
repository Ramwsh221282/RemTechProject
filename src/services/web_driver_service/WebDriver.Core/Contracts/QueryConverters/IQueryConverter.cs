using RemTech.WebDriver.Plugin.Queries;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.WebDriver.Plugin.Contracts.QueryConverters;

internal interface IQueryConverter
{
    public Result<IQuery<TResult>> Convert<TQuery, TResult>(WebDriverRequest? request);
}
