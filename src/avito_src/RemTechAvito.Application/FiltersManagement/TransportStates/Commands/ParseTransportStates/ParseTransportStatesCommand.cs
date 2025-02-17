using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Core.FiltersManagement.TransportStates;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
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
        IAsyncEnumerable<Result<TransportState>> states = parser.Parse(ct);
        int count = 0;
        await foreach (var state in states)
        {
            if (state.IsFailure)
            {
                logger.Error(
                    "{Command} state is invalid: {Error}",
                    nameof(ParseTransportStatesCommand),
                    state.Error.Description
                );
                continue;
            }

            Result insertion = await repository.Add(state, ct);
            if (insertion.IsFailure)
                continue;

            count++;
        }

        logger.Information(
            "{Command} parsing completed. Parsed results: {Count}.",
            nameof(ParseTransportStatesCommand),
            count
        );

        return Result.Success();
    }
}
