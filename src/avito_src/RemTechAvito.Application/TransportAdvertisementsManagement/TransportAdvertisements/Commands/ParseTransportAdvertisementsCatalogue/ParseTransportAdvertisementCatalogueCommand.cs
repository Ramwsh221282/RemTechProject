using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Core.Common.ValueObjects;
using RemTechAvito.Core.ParserJournalManagement;
using RemTechAvito.Infrastructure.Contracts.Parser.AdvertisementsParsing;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.ParseTransportAdvertisementsCatalogue;

public sealed record ParseTransportAdvertisementCatalogueCommand(
    string CatalogueUrl,
    IEnumerable<long>? identefiers = null,
    IEnumerable<string>? additions = null
) : IAvitoCommand
{
    internal static void Register(IServiceCollection services)
    {
        services.AddScoped<
            IAvitoCommandHandler<ParseTransportAdvertisementCatalogueCommand>,
            ParseTransportAdvertisementsCatalogueCommandHandler
        >();
    }
}

internal sealed class ParseTransportAdvertisementsCatalogueCommandHandler(
    ITransportAdvertisementsCommandRepository advertisementsRepository,
    IParserJournalCommandRepository journalsRepository,
    IAdvertisementCatalogueParser parser,
    ILogger logger
) : IAvitoCommandHandler<ParseTransportAdvertisementCatalogueCommand>
{
    public Task<Result> Handle(
        ParseTransportAdvertisementCatalogueCommand command,
        CancellationToken ct = default
    )
    {
        return Task.Run(
            async () =>
            {
                logger.Information(
                    "{Command} invoked.",
                    nameof(ParseTransportAdvertisementCatalogueCommand)
                );
                var addedData = 0;

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var parsedData = parser.Parse(
                    command.CatalogueUrl,
                    existingIds: command.identefiers,
                    additions: command.additions,
                    ct: ct
                );
                await foreach (var item in parsedData)
                {
                    var advertisement = item.ToTransportAdvertisement();
                    if (advertisement.IsFailure)
                    {
                        logger.Warning(
                            "Skipped advertisement. Error: {Error}",
                            advertisement.Error.Description
                        );
                        continue;
                    }

                    var id = await advertisementsRepository.Add(advertisement, ct);
                    if (id == Guid.Empty)
                        continue;
                    addedData++;
                    logger.Information(
                        "{Command} parsing completed. Parsed results: {Count}.",
                        nameof(ParseTransportAdvertisementCatalogueCommand),
                        addedData
                    );
                }

                stopwatch.Stop();
                var time = Time.Create(stopwatch.Elapsed);
                var journal =
                    addedData == 0
                        ? ParserJournal.CreateFailure(
                            time,
                            "Failed at parsing.",
                            addedData,
                            command.CatalogueUrl
                        )
                        : ParserJournal.CreateSuccess(
                            time,
                            addedData,
                            command.CatalogueUrl,
                            "Success at parsing."
                        );
                await journalsRepository.Add(journal, ct);
                return journal.IsSuccess ? Result.Success() : new Error(journal.ErrorMessage);
            },
            ct
        );
    }
}
