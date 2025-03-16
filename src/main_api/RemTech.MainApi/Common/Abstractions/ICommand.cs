using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.Common.Abstractions;

public interface ICommand<TCommandResult>;

public interface ICommandHandler<TCommand, TCommandResult>
    where TCommand : ICommand<TCommandResult>
{
    public Task<Result<TCommandResult>> Handle(TCommand command, CancellationToken ct = default);
}
