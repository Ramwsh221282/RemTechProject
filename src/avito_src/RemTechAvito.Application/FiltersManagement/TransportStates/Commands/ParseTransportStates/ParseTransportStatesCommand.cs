using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Core.FiltersManagement.TransportStates;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.FiltersManagement.TransportStates.Commands.ParseTransportStates;

public sealed record ParseTransportStatesCommand : IAvitoCommand;

public sealed class ParseTransportStatesCommandHandler(
    ITransportStatesParser parser,
    ITransportStatesRepository repository,
    ILogger logger
) : IAvitoCommandHandler<ParseTransportStatesCommand>
{
    public async Task<Result> Handle(
        ParseTransportStatesCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information("{Command} invoked.", nameof(ParseTransportStatesCommand));

        Result<TransportStatesCollection> collection = await parser.Parse(ct);
        if (collection.IsFailure)
            return collection;

        Result insertion = await repository.Add(collection, ct);
        if (insertion.IsFailure)
            return insertion;

        logger.Information(
            "{Command} parsing completed. Parsed results: {Count}.",
            nameof(ParseTransportStatesCommand),
            collection.Value.Count
        );

        return insertion;
    }
}
