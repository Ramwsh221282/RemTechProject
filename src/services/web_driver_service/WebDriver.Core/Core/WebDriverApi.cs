using RemTech.WebDriver.Plugin.Commands;
using RemTech.WebDriver.Plugin.Queries;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.WebDriver.Plugin.Core;

internal sealed class WebDriverApi
{
    private readonly WebDriverDispatcher _dispatcher;

    public WebDriverApi(WebDriverDispatcher dispatcher) => _dispatcher = dispatcher;

    public async Task<Result> ExecuteCommand<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        Result result = await _dispatcher.Dispatch(command);
        return result;
    }

    public async Task<Result<TResult>> ExecuteQuery<TQuery, TResult>(TQuery query)
        where TQuery : IQuery<TResult>
    {
        Result<TResult> result = await _dispatcher.Dispatch<TQuery, TResult>(query);
        return result;
    }
}
