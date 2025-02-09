using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Core.Core;

namespace WebDriver.Core.Queries.GetElement;

public sealed class GetElementQueryHandler
    : BaseWebDriverHandler,
        IWebDriverQueryHandler<GetElementQuery, WebElementObject>
{
    public GetElementQueryHandler(WebDriverInstance instance, ILogger logger)
        : base(instance, logger) { }

    public async Task<Result<WebElementObject>> Execute(GetElementQuery query)
    {
        Result<WebElementObject> result = _instance.GetSingleElement(query);
        if (result.IsFailure)
        {
            Error error = result.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information(
            "Got element with path: {Path} and type {Type}",
            query.Path,
            query.Type
        );
        return await Task.FromResult(result);
    }
}
