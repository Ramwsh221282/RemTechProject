using Microsoft.Extensions.DependencyInjection;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Commands;
using WebDriver.Core.Queries;

namespace WebDriver.Core.Core;

public sealed class WebDriverDispatcher
{
    private readonly IServiceScopeFactory _factory;

    public WebDriverDispatcher(IServiceScopeFactory factory) => _factory = factory;

    public async Task<Result> Dispatch<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        using IServiceScope scope = _factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        IWebDriverCommandHandler<TCommand> handler = provider.GetRequiredService<
            IWebDriverCommandHandler<TCommand>
        >();
        return await handler.Handle(command);
    }

    public async Task<Result<TResult>> Dispatch<TQuery, TResult>(TQuery query)
        where TQuery : IQuery<TResult>
    {
        using IServiceScope scope = _factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        IWebDriverQueryHandler<TQuery, TResult> handler = provider.GetRequiredService<
            IWebDriverQueryHandler<TQuery, TResult>
        >();
        return await handler.Execute(query);
    }
}
