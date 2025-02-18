using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;

public sealed record ParseTransportTypesCommand : IAvitoCommand;

internal sealed class ParseTransportTypesCommandHandler(
    ITransportTypesParser parser,
    ITransportTypesRepository repository,
    ILogger logger
) : IAvitoCommandHandler<ParseTransportTypesCommand>
{
    public async Task<Result> Handle(
        ParseTransportTypesCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information("{Command} invoked.", nameof(ParseTransportTypesCommand));

        IAsyncEnumerable<Result<TransportType>> types = parser.Parse(ct);
        int count = 0;
        await foreach (var type in types)
        {
            if (type.IsFailure)
            {
                logger.Error(
                    "{Command}. Error: {Text}",
                    nameof(ParseTransportTypesCommand),
                    type.Error.Description
                );
                continue;
            }

            Result saving = await repository.Add(type);
            if (saving.IsFailure)
                continue;
            count++;
        }

        logger.Information(
            "{Command} parsing completed. Parsed results: {Count}.",
            nameof(ParseTransportTypesCommand),
            count
        );

        return Result.Success();
    }
}
