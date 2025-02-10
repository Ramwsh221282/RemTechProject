using FluentValidation.Results;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.Handlers;

namespace WebDriver.Application;

public sealed class WebDriverApi(WebDriverDispatcher dispatcher, ILogger logger)
{
    public async Task<Result> ExecuteCommand<TCommand>(TCommand command)
        where TCommand : IWebDriverCommand
    {
        if (await dispatcher.IsCommandValidatorExists<TCommand>())
        {
            ValidationResult validation = await dispatcher.ValidateCommand(command);
            if (!validation.IsValid)
                return LogErrorAndReturn(validation.ToError());
        }

        Result result = await dispatcher.Dispatch(command);
        return result;
    }

    public async Task<Result<TResult>> ExecuteQuery<TQuery, TResult>(TQuery query)
        where TQuery : IWebDriverQuery<TResult>
    {
        if (await dispatcher.IsQueryValidatorExists<TQuery, TResult>())
        {
            ValidationResult validation = await dispatcher.ValidateQuery<TQuery, TResult>(query);
            if (!validation.IsValid)
                return LogErrorAndReturn(validation.ToError());
        }

        Result<TResult> result = await dispatcher.Dispatch<TQuery, TResult>(query);
        return result;
    }

    private Error LogErrorAndReturn(Error error)
    {
        logger.Error("{Error}", error.Description);
        return error;
    }
}
