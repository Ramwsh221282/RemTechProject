using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Application.Abstractions.Handlers;

public interface IAvitoCommand;

public interface IAvitoCommand<TResult>;

public interface IAvitoCommandHandler<in TCommand>
    where TCommand : class, IAvitoCommand
{
    Task<Result> Handle(TCommand command, CancellationToken ct = default);
}

public interface IAvitoCommandHandler<in TCommand, TResult>
    where TCommand : class, IAvitoCommand<TResult>
{
    Task<Result<TResult>> Handle(TCommand command, CancellationToken ct = default);
}
