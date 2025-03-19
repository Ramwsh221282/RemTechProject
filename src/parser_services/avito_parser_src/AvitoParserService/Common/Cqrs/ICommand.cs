namespace AvitoParserService.Common.Cqrs;

public interface ICommand<TCommandResult>;

public interface ICommandHandler<TCommand, TCommandResult>
    where TCommand : ICommand<TCommandResult>
{
    public Task<TCommandResult> Handle(TCommand command);
}
