using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Core.FiltersManagement.CustomerTypes;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.FiltersManagement.CustomerTypes.Commands.ParseCustomerTypes;

public record ParseCustomerTypesCommand : IAvitoCommand;

public sealed class ParseCustomerTypesCommandHandler(
    ICustomerTypesParser parser,
    ICustomerTypesRepository repository,
    ILogger logger
) : IAvitoCommandHandler<ParseCustomerTypesCommand>
{
    public async Task<Result> Handle(
        ParseCustomerTypesCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information("{Command} invoked.", nameof(ParseCustomerTypesCommandHandler));

        Result<CustomerTypesCollection> collection = await parser.Parse(ct);
        if (collection.IsFailure)
            return collection;

        Result insertion = await repository.Add(collection, ct);
        if (insertion.IsFailure)
            return insertion;

        logger.Information(
            "{Command} parsing completed. Parsed results: {Count}.",
            nameof(ParseCustomerTypesCommandHandler),
            collection.Value.Count
        );

        return insertion;
    }
}
