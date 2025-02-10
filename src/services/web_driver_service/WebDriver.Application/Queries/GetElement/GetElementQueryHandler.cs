using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.SearchStrategies;
using WebDriver.Core.Models.SearchStrategies.Implementations;

namespace WebDriver.Application.Queries.GetElement;

public sealed record GetElementQuery(ElementPathDataDTO Data) : IWebDriverQuery<WebElementObject>;

internal sealed class GetElementQueryHandler(WebDriverInstance instance, ILogger logger)
    : BaseWebDriverHandler(instance, logger),
        IWebDriverQueryHandler<GetElementQuery, WebElementObject>
{
    public async Task<Result<WebElementObject>> Execute(GetElementQuery query)
    {
        ElementPathDataDTO data = query.Data;
        ISingleElementSearchStrategy strategy = ElementSearchStrategyFactory.CreateForNew(
            data.Path,
            data.Type
        );
        Result<WebElementObject> element = await _instance.FindElement(strategy);
        if (element.IsFailure)
        {
            Error error = element.Error;
            _logger.Error(error.Description);
            return error;
        }

        _logger.Information("Got element with path: {Path} and type {Type}", data.Path, data.Type);
        return await Task.FromResult(element);
    }
}
