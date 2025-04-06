namespace RemTech.Shared.SDK.CqrsPattern.Commands;

public interface ICommandHandler<in TCommand, TCommandResult>
    where TCommand : ICommand<TCommandResult>
{
    Task<TCommandResult> Handle(TCommand command, CancellationToken ct = default);
}
