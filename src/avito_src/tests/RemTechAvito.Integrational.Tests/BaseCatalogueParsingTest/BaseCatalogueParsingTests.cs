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

            IAsyncEnumerable<ParsedTransportAdvertisement> advertisements = parser.Parse(url, ct);
            int count = 0;
            await foreach (ParsedTransportAdvertisement advertisement in advertisements)
            {
                _logger.Information("Total: {Count}", count);
                count++;
            }
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

    [Fact]
    public async Task Invoke_Multiple_Pages_Sample()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        string[] pages =
        [
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/hifoune-ASgBAgICAkRU4E3cxg2WkdsR",
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/bull-ASgBAgICAkRU4E3cxg3orT8",
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/lugong-ASgBAgICAkRU4E3cxg2y0mY",
        ];

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        Stopwatch sw = new Stopwatch();
        sw.Start();

        foreach (var page in pages)
        {
            try
            {
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
                IAsyncEnumerable<ParsedTransportAdvertisement> advertisements = parser.Parse(
                    page,
                    ct
                );
                int count = 0;

                await foreach (ParsedTransportAdvertisement advertisement in advertisements)
                {
                    _logger.Information("Total: {Count}", count);
                    count++;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal("FATAL. Catalogue: {Url}. Exception: {Ex}", page, ex.Message);
            }
            finally
            {
                SingleCommunicationPublisher stopper = new SingleCommunicationPublisher(
                    queue,
                    host,
                    user,
                    password
                );
                await stopper.Send(new StopWebDriverContract(), ct);
            }
        }

        sw.Stop();
        _logger.Information("Time elapsed: {Time}", sw.Elapsed.Minutes);

        await worker.StopAsync(ct);
    }
}
