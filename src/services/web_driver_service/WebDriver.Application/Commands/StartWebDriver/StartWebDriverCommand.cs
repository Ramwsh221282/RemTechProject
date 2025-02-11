using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using Result = RemTechCommon.Utils.ResultPattern.Result;

namespace WebDriver.Application.Commands.StartWebDriver;

public sealed record StartWebDriverCommand(DriverStartDataDTO Data) : IWebDriverCommand;

internal sealed class StartWebDriverCommandHandler(WebDriverInstance instance, ILogger logger)
    : IWebDriverCommandHandler<StartWebDriverCommand>
{
    public async Task<Result> Handle(StartWebDriverCommand command)
    {
        DriverStartDataDTO data = command.Data;
        Result starting = instance.StartWebDriver(data.Strategy);
        if (starting.IsFailure)
            return starting.LogAndReturn(logger);

        logger.Information("Web Driver Has been started with strategy: {Strategy}", data.Strategy);
        return await Task.FromResult(starting);
    }
}
