using FluentValidation.Results;
using RemTech.Parser.Contracts.Contracts.Commands;
using RemTech.Parser.Implementation.Core;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Commands.OpenPage;

public sealed class OpenPageCommandHandler
    : BaseWebDriverCommand,
        IWebDriverCommandHandler<OpenPageCommand>
{
    private readonly OpenPageCommandValidator _validator;

    public OpenPageCommandHandler(
        WebDriverInstance instance,
        ILogger logger,
        OpenPageCommandValidator validator
    )
        : base(instance, logger) => _validator = validator;

    public async Task<Result> Handle(OpenPageCommand command)
    {
        if (_instance.Instance == null || _instance.IsDisposed)
            return WebDriverPluginErrors.Disposed.LogAndReturn(_logger);

        ValidationResult result = await _validator.ValidateAsync(command);
        if (!result.IsValid)
            return result.ToError().LogAndReturn(_logger);

        await _instance.Instance.Navigate().GoToUrlAsync(command.PageUrl);
        string url = _instance.Instance.Url;

        if (url == command.PageUrl)
            return Result
                .Success()
                .LogAndReturn(_logger, $"Opened page with url: {command.PageUrl}");

        Error error = new Error("Opened page url is different from command page url.");
        return error.LogAndReturn(_logger);
    }
}
