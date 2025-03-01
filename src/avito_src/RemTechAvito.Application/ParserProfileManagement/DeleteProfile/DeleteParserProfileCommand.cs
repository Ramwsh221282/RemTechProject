using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.ParserProfileManagement.DeleteProfile;

public sealed record DeleteParserProfileCommand(string? Id) : IAvitoCommand
{
    internal static void Register(IServiceCollection services)
    {
        services.AddScoped<
            IAvitoCommandHandler<DeleteParserProfileCommand>,
            DeleteParserProfileCommandHandler
        >();
    }
}

internal sealed class DeleteParserProfileCommandHandler(
    IParserProfileCommandRepository repository,
    ILogger logger
) : IAvitoCommandHandler<DeleteParserProfileCommand>
{
    public async Task<Result> Handle(
        DeleteParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information(
            "{Command} requested. Body: {Body}",
            nameof(DeleteParserProfileCommand),
            command
        );
        var deletion = await repository.Delete(command.Id, ct);
        return deletion;
    }
}
