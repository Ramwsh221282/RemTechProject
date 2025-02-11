using FluentValidation.Results;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Queries.GetElementAttribute;

public sealed record GetElementAttributeQuery(
    ExistingElementDTO ExistingData,
    ElementAttributeDTO AttributeData
) : IWebDriverQuery<string>;

internal sealed class GetElementAttributeValueQueryHandler(
    WebDriverInstance instance,
    ILogger logger,
    ExistingElementDTOValidator existingValidator,
    ElementAttributeDTOValidator attributeValidator
) : IWebDriverQueryHandler<GetElementAttributeQuery, string>
{
    public async Task<Result<string>> Execute(GetElementAttributeQuery query)
    {
        ExistingElementDTO existingData = query.ExistingData;
        ValidationResult existingValidation = await existingValidator.ValidateAsync(existingData);
        if (!existingValidation.IsValid)
            return existingValidation.LogAndReturn(logger);

        ElementAttributeDTO attributeData = query.AttributeData;
        ValidationResult attributeValidation = await attributeValidator.ValidateAsync(
            attributeData
        );
        if (!attributeValidation.IsValid)
            return attributeValidation.LogAndReturn(logger);

        IInteractionStrategy<string> strategy = InteractionStrategyFactory.ExtractAttribute(
            existingData.ExistingId,
            attributeData.Attribute
        );

        Result<string> attribute = await instance.PerformInteraction(strategy);
        if (attribute.IsFailure)
            return attribute.LogAndReturn(logger);

        logger.Information(
            "Got attribute value ({Name} {AttributeName}) of element({Id})",
            attributeData.Attribute,
            attribute.Value,
            existingData.ExistingId
        );
        return attribute;
    }
}
