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
    ITransportTypesCommandRepository commandRepository,
    ILogger logger
) : IAvitoCommandHandler<ParseTransportTypesCommand>
{
    public async Task<Result> Handle(
        ParseTransportTypesCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information("{Command} invoked.", nameof(ParseTransportTypesCommand));

        var types = parser.Parse(ct);
        var count = 0;
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

            var saving = await commandRepository.Add(type);
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
