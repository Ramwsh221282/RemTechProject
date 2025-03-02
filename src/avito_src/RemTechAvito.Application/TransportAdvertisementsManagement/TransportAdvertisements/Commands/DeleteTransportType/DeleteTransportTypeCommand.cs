using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Injections;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.DeleteTransportType;

public sealed record DeleteTransportTypeCommand(RemoveUserTransportTypeQuery Query) : IAvitoCommand
{
    internal static void Register(IServiceCollection services)
    {
        services.AddScoped<
            IAvitoCommandHandler<DeleteTransportTypeCommand>,
            DeleteTransportTypeCommandHandler
        >();
    }
}

internal sealed class DeleteTransportTypeCommandHandler(
    ILogger logger,
    ITransportTypesCommandRepository repository
) : IAvitoCommandHandler<DeleteTransportTypeCommand>
{
    public async Task<Result> Handle(
        DeleteTransportTypeCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information(
            "{Command} called for {Name}",
            nameof(DeleteTransportTypeCommand),
            command.Query.Name
        );
        var deletion = await repository.Delete(command.Query, ct);
        if (deletion.IsFailure)
        {
            var error = deletion.Error;
            return error.LogAndReturnError(logger, nameof(DeleteTransportTypeCommand));
        }

        return deletion;
    }
}
