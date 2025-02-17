using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Core.FiltersManagement.CustomerStates;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.FiltersManagement.CustomerStates.Commands.ParseCustomerStates;

public sealed record ParseCustomerStatesCommand : IAvitoCommand;

public sealed class ParseCustomerStatesCommandHandler(
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

        Result<CustomerStatesCollection> collection = await parser.Parse(ct);
        if (collection.IsFailure)
            return collection;

        Result insertion = await repository.Add(collection, ct);
        if (insertion.IsFailure)
            return insertion;

        logger.Information(
            "{Command} parsing completed. Parsed results: {Count}.",
            nameof(ParseCustomerStatesCommandHandler),
            collection.Value.Count
        );

        return insertion;
    }
}
