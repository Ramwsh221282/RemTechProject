using Microsoft.Extensions.DependencyInjection;
using RemTech.Parser.Contracts.Contracts;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebDriverDispatcher
{
    private readonly IServiceScopeFactory _factory;

    public WebDriverDispatcher(IServiceScopeFactory factory) => _factory = factory;

    public async Task<Result> Dispatch<TCommand>(TCommand command)
        where TCommand : IWebDriverCommand
    {
        using var scope = _factory.CreateScope();

        IWebDriverCommandHandler<TCommand> handler = scope.ServiceProvider.GetRequiredService<
            IWebDriverCommandHandler<TCommand>
        >();

        return await handler.Handle(command);
    }

    public async Task<Result<TResult>> Dispatch<TQuery, TResult>(TQuery query)
        where TQuery : IWebDriverQuery<TResult>
    {
        using var scope = _factory.CreateScope();

        IWebDriverQueryHandler<TQuery, TResult> handler = scope.ServiceProvider.GetRequiredService<
            IWebDriverQueryHandler<TQuery, TResult>
        >();

        return await handler.Execute(query);
    }
}
