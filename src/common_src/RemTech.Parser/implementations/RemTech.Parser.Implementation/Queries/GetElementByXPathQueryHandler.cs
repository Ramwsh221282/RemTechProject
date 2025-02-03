using FluentValidation.Results;
using RemTech.Parser.Contracts.Contracts;
using RemTech.Parser.Contracts.Contracts.Queries;
using RemTech.Parser.Implementation.Core;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Queries;

public sealed class GetElementByXPathQueryHandler
    : BaseWebDriverHandler,
        IWebDriverQueryHandler<GetElementQuery, WebElementObject>
{
    private readonly GetElementByXPathQueryValidator _validator;

    public GetElementByXPathQueryHandler(
        WebDriverInstance instance,
        ILogger logger,
        GetElementByXPathQueryValidator validator
    )
        : base(instance, logger) => _validator = validator;

    public async Task<Result<WebElementObject>> Execute(GetElementQuery query)
    {
        if (_instance.Instance == null || _instance.IsDisposed)
            return WebDriverPluginErrors.Disposed.LogAndReturn(_logger);

        ValidationResult validation = await _validator.ValidateAsync(query);
        if (!validation.IsValid)
            return validation.ToError().LogAndReturn(_logger);

        Result<WebElementObject> result = _instance.GetElement(query);
        if (result.IsFailure)
            return result.Error.LogAndReturn(_logger);

        _logger.Information(
            "Got element with path: {Path} and type {Type}",
            query.Path,
            query.Type
        );

        return await Task.FromResult(result);
    }
}
