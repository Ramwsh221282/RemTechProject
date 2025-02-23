using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.ParserProfileManagement.DeleteProfile;

public sealed record DeleteParserProfileCommand(string? Id) : IAvitoCommand;

internal sealed class DeleteParserProfileCommandHandler
    : IAvitoCommandHandler<DeleteParserProfileCommand>
{
    private readonly IParserProfileCommandRepository _repository;
    private readonly ILogger _logger;

    public DeleteParserProfileCommandHandler(
        IParserProfileCommandRepository repository,
        ILogger logger
    )
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result> Handle(
        DeleteParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        _logger.Information(
            "{Command} requested. Body: {Body}",
            nameof(DeleteParserProfileCommand),
            command
        );
        Result<Guid> deletion = await _repository.Delete(command.Id, ct);
        return deletion;
    }
}
