using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Contracts.Common.Responses.TransportTypesManagement;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;

public sealed record ParseTransportTypesCommand : IAvitoCommand<TransportTypeResponse>;

internal sealed class ParseTransportTypesCommandHandler(
    ITransportTypesParser parser,
    ITransportTypesCommandRepository repository,
    ILogger logger
) : IAvitoCommandHandler<ParseTransportTypesCommand, TransportTypeResponse>
{
    public async Task<Result<TransportTypeResponse>> Handle(
        ParseTransportTypesCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information("{Command} invoked.", nameof(ParseTransportTypesCommand));

        var types = parser.Parse(ct);
        List<TransportType> results = [];
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

            results.Add(type);
        }

        var insertion = await repository.Add(results, ct);
        if (insertion.IsFailure)
            return insertion.Error;

        logger.Information(
            "{Command} parsing completed. Parsed results: {Count}.",
            nameof(ParseTransportTypesCommand),
            results.Count
        );

        var items = results.Select(t => new TransportTypeDto(t.Name, t.Link));
        return new TransportTypeResponse(items, results.Count);
    }
}
