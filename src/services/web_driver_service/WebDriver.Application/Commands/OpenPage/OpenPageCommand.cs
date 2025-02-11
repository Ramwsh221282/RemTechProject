using FluentValidation;
using FluentValidation.Results;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Commands.OpenPage;

public sealed record OpenPageCommand(WebPageDataDTO Data) : IWebDriverCommand;

internal sealed class OpenPageCommandHandler(
    WebDriverInstance instance,
    ILogger logger,
    WebPageDataDTOValidator validator
) : IWebDriverCommandHandler<OpenPageCommand>
{
    public async Task<Result> Handle(OpenPageCommand command)
    {
        WebPageDataDTO data = command.Data;
        ValidationResult validation = await validator.ValidateAsync(data);
        if (!validation.IsValid)
            return validation.LogAndReturn(logger);

        IInteractionStrategy<string> strategy = InteractionStrategyFactory.CreateOpenPage(
            data.PageUrl
        );

        Result<string> opening = await instance.PerformInteraction(strategy);
        if (opening.IsFailure)
            return opening.LogAndReturn(logger);

        logger.Information("Web Driver Has opened page with url: {Url}", data.PageUrl);
        return opening;
    }
}
