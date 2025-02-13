using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;

namespace WebDriver.Application.Commands.ClearPool;

public record ClearPoolCommand : IWebDriverCommand;

internal sealed class ClearPoolCommandHandler(WebDriverInstance instance, ILogger logger)
    : IWebDriverCommandHandler<ClearPoolCommand>
{
    public async Task<Result> Handle(ClearPoolCommand command)
    {
        instance.RefreshPool();
        logger.Information("Cleared web driver elements pool");
        return await Task.FromResult(Result.Success());
    }
}
