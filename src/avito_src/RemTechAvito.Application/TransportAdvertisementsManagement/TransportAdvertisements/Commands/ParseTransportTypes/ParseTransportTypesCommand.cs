using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Contracts.Common.Responses.TransportTypesManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;

public sealed record ParseTransportTypesCommand : IAvitoCommand<TransportTypeResponse>
{
    internal static void Register(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<
            IAvitoCommandHandler<ParseTransportTypesCommand, TransportTypeResponse>,
            ParseTransportTypesCommandHandler
        >();
    }
}

internal sealed class ParseTransportTypesCommandHandler(
    ITransportTypesParser parser,
    ITransportTypesCommandRepository commandRepository,
    ITransportTypesQueryRepository queryRepository,
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
        var results = new List<SystemTransportType>();

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

        var existingLinks = await queryRepository.Get(
            new GetTransportTypesQuery(
                Links: results.Select(r => new GetTransportTypeLinkCondition(r.Link)).ToArray()
            ),
            ct
        );

        var newTypes = results.Where(r => !existingLinks.Items.Any(i => i.Link == r.Link)).ToList();

        if (newTypes.Count == 0)
        {
            logger.Information(
                "{Command} no new transport types found.",
                nameof(ParseTransportTypesCommand)
            );
            return new TransportTypeResponse([], results.Count);
        }

        var insertion = await commandRepository.AddMany(newTypes, ct);
        if (insertion.IsFailure)
            return insertion.Error;

        logger.Information(
            "{Command} parsing completed. Parsed results: {Count}. New types added: {NewCount}.",
            nameof(ParseTransportTypesCommand),
            results.Count,
            newTypes.Count
        );

        var items = newTypes.Select(t => new TransportTypeDto(t.Name, t.Link));
        return new TransportTypeResponse(items, results.Count);
    }
}
