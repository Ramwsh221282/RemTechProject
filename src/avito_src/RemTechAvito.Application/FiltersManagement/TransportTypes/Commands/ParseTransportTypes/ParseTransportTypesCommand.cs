using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;

public sealed record ParseTransportTypesCommand : IAvitoCommand;

public sealed class ParseTransportTypesCommandHandler(
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

        Result<TransportTypesCollection> collection = await parser.Parse(ct);
        if (collection.IsFailure)
            return collection;

        Result insertion = await repository.Add(collection, ct);
        if (insertion.IsFailure)
            return insertion;

        logger.Information(
            "{Command} parsing completed. Parsed results: {Count}.",
            nameof(ParseTransportTypesCommand),
            collection.Value.Count
        );

        return insertion;
    }
}
