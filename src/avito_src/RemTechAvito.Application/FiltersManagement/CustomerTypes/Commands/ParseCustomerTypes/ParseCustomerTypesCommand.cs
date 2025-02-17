using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Core.FiltersManagement.CustomerTypes;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
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
        IAsyncEnumerable<Result<CustomerType>> types = parser.Parse(ct);
        int count = 0;

        await foreach (var type in types)
        {
            if (type.IsFailure)
            {
                logger.Error(
                    "{Command} invalid type {Type}",
                    nameof(ParseCustomerTypesCommand),
                    type
                );
                continue;
            }

            Result inserting = await repository.Add(type, ct);
            if (inserting.IsFailure)
                continue;

            count++;
        }

        logger.Information(
            "{Command} parsing completed. Parsed results: {Count}.",
            nameof(ParseCustomerTypesCommandHandler),
            count
        );
        return Result.Success();
    }
}
