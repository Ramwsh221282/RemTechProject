using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing;
using WebDriver.Worker.Service;

namespace RemTechAvito.Integrational.Tests.BaseCatalogueParsingTest;

public sealed class BaseCatalogueParsingTests : BasicParserTests
{
    [Fact]
    public async Task Invoke_Basic_Catalogue_Parsing_Test()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        const string url =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/liugong-ASgBAgICAkRU4E3cxg3urj8";

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        IAdvertisementCatalogueParser parser = new AdvertisementCatalogueParser(publisher, _logger);

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        Stopwatch sw = new Stopwatch();
        sw.Start();

        await parser.Parse(url, ct);

        sw.Stop();
        _logger.Information("Time elapsed: {Time}", sw.Elapsed.Minutes);

        await worker.StopAsync(ct);
    }
}
