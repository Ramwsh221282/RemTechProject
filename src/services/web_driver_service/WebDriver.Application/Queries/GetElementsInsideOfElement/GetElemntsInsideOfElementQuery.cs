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

namespace WebDriver.Application.Queries.GetElementsInsideOfElement;

public record GetElementsInsideOfElementQuery(ExistingElementDTO Existing, ElementPathDataDTO Data)
    : IWebDriverQuery<WebElementObject[]>;

internal sealed class GetElementsInsideOfElementQueryHandler(
    WebDriverInstance instance,
    ILogger logger,
    ExistingElementDTOValidator existingValidator,
    ElementPathDataDTOValidator pathValidator
) : IWebDriverQueryHandler<GetElementsInsideOfElementQuery, WebElementObject[]>
{
    public async Task<Result<WebElementObject[]>> Execute(GetElementsInsideOfElementQuery query)
    {
        ExistingElementDTO dataExisting = query.Existing;
        ValidationResult existingValidation = await existingValidator.ValidateAsync(dataExisting);
        if (!existingValidation.IsValid)
            return existingValidation.LogAndReturn(logger);

        ElementPathDataDTO dataPath = query.Data;
        ValidationResult dataValidation = await pathValidator.ValidateAsync(dataPath);
        if (!dataValidation.IsValid)
            return dataValidation.LogAndReturn(logger);

        IMultipleElementSearchStrategy strategy =
            ElementSearchStrategyFactory.CreateForMultipleChilds(
                dataExisting.ExistingId,
                dataPath.Path,
                dataPath.Type
            );

        Result<WebElementObject[]> elements = await instance.FindElements(strategy);
        if (elements.IsFailure)
            return elements.LogAndReturn(logger);

        foreach (var element in elements.Value)
        {
            IInteractionStrategy<string> initializeHTML = InteractionStrategyFactory.ExtractHtml(
                element.ElementId
            );
            await instance.PerformInteraction(initializeHTML);

            IInteractionStrategy<string> initializeText = InteractionStrategyFactory.ExtractText(
                element.ElementId
            );
            await instance.PerformInteraction(initializeText);
        }

        logger.Information(
            "Found elements({ChildPath} {ChildType}) of parent({ParentId})",
            dataPath.Path,
            dataPath.Type,
            dataExisting.ExistingId
        );
        return elements;
    }
}
