using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Contracts.Parser.AdvertisementsParsing;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.ParseTransportAdvertisementsCatalogue;

public sealed record ParseTransportAdvertisementCatalogueCommand(string CatalogueUrl)
    : IAvitoCommand;

public sealed class ParseTransportAdvertisementsCatalogueCommandHandler(
    ITransportAdvertisementsCommandRepository commandRepository,
    IAdvertisementCatalogueParser parser,
    ILogger logger
) : IAvitoCommandHandler<ParseTransportAdvertisementCatalogueCommand>
{
    public async Task<Result> Handle(
        ParseTransportAdvertisementCatalogueCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information(
            "{Command} invoked.",
            nameof(ParseTransportAdvertisementCatalogueCommand)
        );

        int addedData = 0;

        var parsedData = parser.Parse(command.CatalogueUrl, ct);
        await foreach (var item in parsedData)
        {
            Result<TransportAdvertisement> advertisement = item.ToTransportAdvertisement();
            if (advertisement.IsFailure)
            {
                logger.Warning(
                    "Skipped advertisement. Error: {Error}",
                    advertisement.Error.Description
                );
                continue;
            }
            Guid id = await commandRepository.Add(advertisement, ct);
            if (id == Guid.Empty)
                continue;
            addedData++;
        }

        logger.Information(
            "{Command} parsing completed. Parsed results: {Count}.",
            nameof(ParseTransportAdvertisementCatalogueCommand),
            addedData
        );

        return Result.Success();
    }
}
