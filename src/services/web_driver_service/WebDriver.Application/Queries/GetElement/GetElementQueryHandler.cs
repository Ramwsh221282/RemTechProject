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

public sealed record GetElementQuery(ElementPathDataDTO Data) : IWebDriverQuery<WebElementObject>;

internal sealed class GetElementQueryHandler(
    WebDriverInstance instance,
    ILogger logger,
    ElementPathDataDTOValidator validator
) : IWebDriverQueryHandler<GetElementQuery, WebElementObject>
{
    public async Task<Result<WebElementObject>> Execute(GetElementQuery query)
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
        Result initialization = await instance.PerformInteraction(initializeHTML);

        IInteractionStrategy<string> initializeText = InteractionStrategyFactory.ExtractText(
            element.Value.ElementId
        );
        initialization = await instance.PerformInteraction(initializeText);

        logger.Information("Got element with path: {Path} and type {Type}", data.Path, data.Type);
        return await Task.FromResult(element);
    }
}
