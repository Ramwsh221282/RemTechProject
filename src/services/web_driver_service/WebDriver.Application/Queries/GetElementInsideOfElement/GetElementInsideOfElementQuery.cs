using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.SearchStrategies;
using WebDriver.Core.Models.SearchStrategies.Implementations;

namespace WebDriver.Application.Queries.GetElementInsideOfElement;

public record GetElementInsideOfElementQuery(ExistingElementDTO Existing, ElementPathDataDTO Data)
    : IWebDriverQuery<WebElementObject>;

internal sealed class GetElementInsideOfElementQueryHandler(
    WebDriverInstance instance,
    ILogger logger
)
    : BaseWebDriverHandler(instance, logger),
        IWebDriverQueryHandler<GetElementInsideOfElementQuery, WebElementObject>
{
    public async Task<Result<WebElementObject>> Execute(GetElementInsideOfElementQuery query)
    {
        ExistingElementDTO existing = query.Existing;
        ElementPathDataDTO data = query.Data;

        ISingleElementSearchStrategy strategy = ElementSearchStrategyFactory.CreateForChild(
            existing.ExistingId,
            data.Path,
            data.Type
        );

        Result<WebElementObject> element = await _instance.FindElement(strategy);
        if (element.IsFailure)
        {
            Error error = element.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information(
            "Children elements (Path: {ChildPath} Type: {ChildType}) found",
            element.Value.ElementPath,
            element.Value.ElementPathType
        );
        return element;
    }
}
