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

    public async Task<ValidationResult> ValidateCommand<TCommand>(TCommand command)
        where TCommand : IWebDriverCommand
    {
        using IServiceScope scope = factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        AbstractValidator<TCommand> validator = provider.GetRequiredService<
            AbstractValidator<TCommand>
        >();
        return await validator.ValidateAsync(command);
    }

    public async Task<ValidationResult> ValidateQuery<TQuery, TResult>(TQuery query)
        where TQuery : IWebDriverQuery<TResult>
    {
        using IServiceScope scope = factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        AbstractValidator<TQuery> validator = provider.GetRequiredService<
            AbstractValidator<TQuery>
        >();
        return await validator.ValidateAsync(query);
    }

    public async Task<bool> IsCommandValidatorExists<TCommand>()
        where TCommand : IWebDriverCommand
    {
        using IServiceScope scope = factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        AbstractValidator<TCommand>? validator = provider.GetService<AbstractValidator<TCommand>>();
        return await Task.FromResult(validator != null);
    }

    public async Task<bool> IsQueryValidatorExists<TQuery, TResult>()
        where TQuery : IWebDriverQuery<TResult>
    {
        using IServiceScope scope = factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        AbstractValidator<TQuery>? validator = provider.GetService<AbstractValidator<TQuery>>();
        return await Task.FromResult(validator != null);
    }
}
