using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.SearchStrategies;
using WebDriver.Core.Models.SearchStrategies.Implementations;

namespace WebDriver.Application.Queries.GetElementsInsideOfElement;

public record GetElementsInsideOfElementQuery(ExistingElementDTO Existing, ElementPathDataDTO Data)
    : IWebDriverQuery<WebElementObject[]>;

internal sealed class GetElementsInsideOfElementQueryHandler(
    WebDriverInstance instance,
    ILogger logger
)
    : BaseWebDriverHandler(instance, logger),
        IWebDriverQueryHandler<GetElementsInsideOfElementQuery, WebElementObject[]>
{
    public async Task<Result<WebElementObject[]>> Execute(GetElementsInsideOfElementQuery query)
    {
        ExistingElementDTO dataExisting = query.Existing;
        ElementPathDataDTO dataPath = query.Data;

        IMultipleElementSearchStrategy strategy =
            ElementSearchStrategyFactory.CreateForMultipleChilds(
                dataExisting.ExistingId,
                dataPath.Path,
                dataPath.Type
            );
        Result<WebElementObject[]> elements = await _instance.FindElements(strategy);
        if (elements.IsFailure)
        {
            Error error = elements.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }
        _logger.Information(
            "Found elements({ChildPath} {ChildType}) of parent({ParentId})",
            dataPath.Path,
            dataPath.Type,
            dataExisting.ExistingId
        );
        return elements;
    }
}
