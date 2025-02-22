﻿using FluentValidation.Results;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;
using WebDriver.Core.Models.SearchStrategies;
using WebDriver.Core.Models.SearchStrategies.Implementations;

namespace WebDriver.Application.Queries.GetElementInsideOfElement;

public record GetElementInsideOfElementQuery(ExistingElementDTO Existing, ElementPathDataDTO Data)
    : IWebDriverQuery<WebElementResponseObject>;

internal sealed class GetElementInsideOfElementQueryHandler(
    WebDriverInstance instance,
    ILogger logger,
    ExistingElementDTOValidator existingValidator,
    ElementPathDataDTOValidator pathValidator
) : IWebDriverQueryHandler<GetElementInsideOfElementQuery, WebElementResponseObject>
{
    public async Task<Result<WebElementResponseObject>> Execute(
        GetElementInsideOfElementQuery query
    )
    {
        ExistingElementDTO existing = query.Existing;
        ValidationResult existingValidation = await existingValidator.ValidateAsync(existing);
        if (!existingValidation.IsValid)
            return existingValidation.LogAndReturn(logger);

        ElementPathDataDTO data = query.Data;
        ValidationResult pathValidation = await pathValidator.ValidateAsync(data);
        if (!pathValidation.IsValid)
            return pathValidation.LogAndReturn(logger);

        ISingleElementSearchStrategy strategy = ElementSearchStrategyFactory.CreateForChild(
            existing.ExistingId,
            data.Path,
            data.Type
        );

        Result<WebElementObject> element = await instance.FindElement(strategy);
        if (element.IsFailure)
            return element.LogAndReturn(logger);

        IInteractionStrategy<string> initializeHTML = InteractionStrategyFactory.ExtractHtml(
            element.Value.ElementId
        );
        Result<string> htmlRequest = await instance.PerformInteraction(initializeHTML);

        IInteractionStrategy<string> initializeText = InteractionStrategyFactory.ExtractText(
            element.Value.ElementId
        );
        Result<string> textRequest = await instance.PerformInteraction(initializeText);

        WebElementResponseObject response = new WebElementResponseObject(
            element.Value.ElementId,
            htmlRequest.IsFailure ? string.Empty : htmlRequest.Value,
            textRequest.IsFailure ? string.Empty : textRequest.Value
        );

        logger.Information("Children elements of parent ({Id}) found", existing.ExistingId);
        return response;
    }
}
