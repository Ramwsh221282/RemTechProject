using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Application.Abstractions.Handlers;

public interface IAvitoCommand;

public interface IAvitoCommandHandler<in TCommand>
    where TCommand : class, IAvitoCommand
{
    Task<Result> Handle(TCommand command, CancellationToken ct = default);
}
