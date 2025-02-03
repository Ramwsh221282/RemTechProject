using RemTech.Parser.Contracts.Contracts;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebDriverApi : IWebDriverApi
{
    private readonly WebDriverDispatcher _dispatcher;

    public WebDriverApi(WebDriverDispatcher dispatcher) => _dispatcher = dispatcher;

    public async Task<Result> ExecuteCommand<TCommand>(TCommand command)
        where TCommand : IWebDriverCommand
    {
        Result result = await _dispatcher.Dispatch(command);
        return result;
    }

    public async Task<Result<TResult>> ExecuteQuery<TQuery, TResult>(TQuery query)
        where TQuery : IWebDriverQuery<TResult>
    {
        Result<TResult> result = await _dispatcher.Dispatch<TQuery, TResult>(query);
        return result;
    }
}
