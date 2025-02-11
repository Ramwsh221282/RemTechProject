using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application.Handlers;

namespace WebDriver.Application;

public sealed class WebDriverDispatcher(IServiceScopeFactory factory)
{
    public async Task<Result> Dispatch<TCommand>(TCommand command)
        where TCommand : IWebDriverCommand
    {
        using IServiceScope scope = factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        IWebDriverCommandHandler<TCommand> handler = provider.GetRequiredService<
            IWebDriverCommandHandler<TCommand>
        >();
        return await handler.Handle(command);
    }

    public async Task<Result<TResult>> Dispatch<TQuery, TResult>(TQuery query)
        where TQuery : IWebDriverQuery<TResult>
    {
        using IServiceScope scope = factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        IWebDriverQueryHandler<TQuery, TResult> handler = provider.GetRequiredService<
            IWebDriverQueryHandler<TQuery, TResult>
        >();
        return await handler.Execute(query);
    }
}
