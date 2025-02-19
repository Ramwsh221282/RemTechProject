using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.ParseTransportAdvertisementsCatalogue;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace RemTechAvito.Integrational.Tests.BaseCatalogueParsingTest;

public sealed class BaseCatalogueParsingTests : BasicParserTests
{
    [Fact]
    public async Task Invoke_Parse_Advertisements_Catalogue_Use_Case()
    {
        bool noException = true;
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        try
        {
            // const string url =
            //     "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/hifoune-ASgBAgICAkRU4E3cxg2WkdsR";

            const string url =
                "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/merlo-ASgBAgICAkRU4E3cxg2Arz8";

            ParseTransportAdvertisementCatalogueCommand command = new(url);
            IAvitoCommandHandler<ParseTransportAdvertisementCatalogueCommand> handler =
                _serviceProvider.GetRequiredService<
                    IAvitoCommandHandler<ParseTransportAdvertisementCatalogueCommand>
                >();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            await handler.Handle(command, ct);
            sw.Stop();
            _logger.Information("Time elapsed: {Time}", sw.Elapsed.Minutes);
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "{Test} stopped. Exception: {Ex}",
                nameof(Invoke_Parse_Advertisements_Catalogue_Use_Case),
                ex.Message
            );
            noException = false;
            SingleCommunicationPublisher stopper = new SingleCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            await stopper.Send(new StopWebDriverContract(), ct);
        }

        await worker.StopAsync(ct);
        Assert.True(noException);
    }
}
