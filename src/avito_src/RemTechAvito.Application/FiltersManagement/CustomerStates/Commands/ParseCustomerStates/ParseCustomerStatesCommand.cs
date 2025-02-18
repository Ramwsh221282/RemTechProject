using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Core.FiltersManagement.CustomerStates;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.FiltersManagement.CustomerStates.Commands.ParseCustomerStates;

public sealed record ParseCustomerStatesCommand : IAvitoCommand;

internal sealed class ParseCustomerStatesCommandHandler(
    ICustomerStatesParser parser,
    ICustomerStatesRepository repository,
    ILogger logger
) : IAvitoCommandHandler<ParseCustomerStatesCommand>
{
    public async Task<Result> Handle(
        ParseCustomerStatesCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information("{Command} invoked.", nameof(ParseCustomerStatesCommandHandler));
        IAsyncEnumerable<Result<CustomerState>> states = parser.Parse(ct);
        int count = 0;

        await foreach (var state in states)
        {
            if (state.IsFailure)
            {
                logger.Error(
                    "{Command} cannot add {State} is invalid",
                    nameof(ParseCustomerStatesCommand),
                    state
                );
                continue;
            }

            Result inserting = await repository.Add(state, ct);
            if (inserting.IsFailure)
                continue;

            count++;
        }

        logger.Information(
            "{Command} parsing completed. Parsed results: {Count}.",
            nameof(ParseCustomerStatesCommand),
            count
        );
        return Result.Success();
    }
}
