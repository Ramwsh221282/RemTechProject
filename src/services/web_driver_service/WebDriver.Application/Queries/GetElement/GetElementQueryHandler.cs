using FluentValidation.Results;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;
using WebDriver.Core.Models.SearchStrategies;
using WebDriver.Core.Models.SearchStrategies.Implementations;

namespace WebDriver.Application.Queries.GetElement;

public sealed record GetElementQuery(ElementPathDataDTO Data)
    : IWebDriverQuery<WebElementResponseObject>;

internal sealed class GetElementQueryHandler(
    WebDriverInstance instance,
    ILogger logger,
    ElementPathDataDTOValidator validator
) : IWebDriverQueryHandler<GetElementQuery, WebElementResponseObject>
{
    public async Task<Result<WebElementResponseObject>> Execute(GetElementQuery query)
    {
        ElementPathDataDTO data = query.Data;
        ValidationResult validation = await validator.ValidateAsync(data);
        if (!validation.IsValid)
            return validation.LogAndReturn(logger);

        ISingleElementSearchStrategy strategy = ElementSearchStrategyFactory.CreateForNew(
            data.Path,
            data.Type
        );

        Result<WebElementObject> element = await instance.FindElement(strategy);
        if (element.IsFailure)
            return element.LogAndReturn(logger);

        IInteractionStrategy<string> initializeHTML = InteractionStrategyFactory.ExtractHtml(
            element.Value.ElementId
        );
        Result<string> outerHTMLRequest = await instance.PerformInteraction(initializeHTML);

        IInteractionStrategy<string> initializeText = InteractionStrategyFactory.ExtractText(
            element.Value.ElementId
        );
        Result<string> innerTextRequest = await instance.PerformInteraction(initializeText);

        WebElementResponseObject response = new WebElementResponseObject(
            element.Value.ElementId,
            outerHTMLRequest.IsFailure ? string.Empty : outerHTMLRequest.Value,
            innerTextRequest.IsFailure ? string.Empty : innerTextRequest.Value
        );

        logger.Information("Got element with path: {Path} and type {Type}", data.Path, data.Type);
        return await Task.FromResult(response);
    }
}
