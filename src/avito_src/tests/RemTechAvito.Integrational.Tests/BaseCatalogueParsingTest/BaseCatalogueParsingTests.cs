using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.ParseTransportAdvertisementsCatalogue;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace RemTechAvito.Integrational.Tests.BaseCatalogueParsingTest;

public sealed class BaseCatalogueParsingTests : BasicParserTests
{
    [Fact]
    public async Task Invoke_Parse_Advertisements_Catalogue_Use_Case_Awaitable()
    {
        var noException = true;
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        try
        {
            const string url =
                "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/ace-ASgBAgICAkRU4E3cxg380GY";

            ParseTransportAdvertisementCatalogueCommand command = new(url);
            var handler = _serviceProvider.GetRequiredService<
                IAvitoCommandHandler<ParseTransportAdvertisementCatalogueCommand>
            >();

            var sw = new Stopwatch();
            sw.Start();
            await handler.Handle(command, ct);
            sw.Stop();
            _logger.Information("Time elapsed: {Time}", sw.Elapsed.Minutes);
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "{Test} stopped. Exception: {Ex}",
                nameof(Invoke_Parse_Advertisements_Catalogue_Use_Case_Awaitable),
                ex.Message
            );
            noException = false;
            var stopper = new SingleCommunicationPublisher(queue, host, user, password);
            await stopper.Send(new StopWebDriverContract(), ct);
        }

        Assert.True(noException);
    }
}
