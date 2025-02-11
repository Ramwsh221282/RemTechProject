using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application.Handlers;

namespace WebDriver.Application;

public sealed class WebDriverApi(WebDriverDispatcher dispatcher)
{
    public async Task<Result> ExecuteCommand<TCommand>(TCommand command)
        where TCommand : IWebDriverCommand => await dispatcher.Dispatch(command);

    public async Task<Result<TResult>> ExecuteQuery<TQuery, TResult>(TQuery query)
        where TQuery : IWebDriverQuery<TResult> =>
        await dispatcher.Dispatch<TQuery, TResult>(query);
}
