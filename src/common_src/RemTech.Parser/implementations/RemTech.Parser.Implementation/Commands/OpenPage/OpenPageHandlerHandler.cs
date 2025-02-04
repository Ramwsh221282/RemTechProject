using FluentValidation.Results;
using RemTech.Parser.Contracts.Contracts.Commands;
using RemTech.Parser.Implementation.Core;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Commands.OpenPage;

public sealed class OpenPageHandlerHandler
    : BaseWebDriverHandler,
        IWebDriverCommandHandler<OpenPageCommand>
{
    private readonly OpenPageCommandValidator _validator;

    public OpenPageHandlerHandler(
        WebDriverInstance instance,
        ILogger logger,
        OpenPageCommandValidator validator
    )
        : base(instance, logger) => _validator = validator;

    public async Task<Result> Handle(OpenPageCommand command)
    {
        if (_instance.Instance == null || _instance.IsDisposed)
        {
            Error error = WebDriverPluginErrors.Disposed;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        ValidationResult result = await _validator.ValidateAsync(command);
        if (!result.IsValid)
        {
            Error error = result.ToError();
            _logger.Error("{Error}", error.Description);
            return error;
        }

        await _instance.Instance.Navigate().GoToUrlAsync(command.PageUrl);
        string url = _instance.Instance.Url;

        if (url != command.PageUrl)
        {
            Error error = new Error("Opened page url is different from command page url.");
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information("Opened page with {Url}", url);
        return Result.Success();
    }
}
