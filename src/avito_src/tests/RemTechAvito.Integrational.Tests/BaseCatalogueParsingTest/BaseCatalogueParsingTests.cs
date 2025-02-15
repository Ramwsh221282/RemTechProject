using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace RemTechAvito.Integrational.Tests.BaseCatalogueParsingTest;

public sealed class BaseCatalogueParsingTests : BasicParserTests
{
    [Fact]
    public async Task Invoke_Basic_Catalogue_Parsing_Test()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        try
        {
            const string url =
                "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/hifoune-ASgBAgICAkRU4E3cxg2WkdsR";

            IMessagePublisher publisher = new MultiCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            IAdvertisementCatalogueParser parser = new AdvertisementCatalogueParser(
                publisher,
                _logger
            );

            Stopwatch sw = new Stopwatch();
            sw.Start();

            await parser.Parse(url, ct);

            sw.Stop();
            _logger.Information("Time elapsed: {Time}", sw.Elapsed.Minutes);
        }
        catch (Exception ex)
        {
            SingleCommunicationPublisher stopper = new SingleCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            await stopper.Send(new StopWebDriverContract(), ct);
            _logger.Fatal(
                "Test running finished FATAL. Message: {Exception}. Source: {Source}",
                ex.Message,
                ex.Source
            );
        }
        finally
        {
            await worker.StopAsync(ct);
        }
    }
}
