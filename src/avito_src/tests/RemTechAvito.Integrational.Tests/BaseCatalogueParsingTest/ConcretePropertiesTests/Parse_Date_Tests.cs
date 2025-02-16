using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Integrational.Tests.BaseCatalogueParsingTest.ConcretePropertiesTests;

public sealed class Parse_Date_Tests : BasicParserTests
{
    [Fact]
    public async Task Parse_Date_Sample_1()
    {
        const string url =
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_hifoune_fd30_2025_4373248650?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiJvSDRrWjg0SElwTlQwQnN2Ijt94UlT_z8AAAA";
        const string path = ".//span[@data-marker='item-view/item-date']";
        const string type = "xpath";
        const string name = "date";

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        try
        {
            IMessagePublisher publisher = new MultiCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            WebElementPool pool = new WebElementPool();
            using WebDriverSession session = new WebDriverSession(publisher);
            await session.ExecuteBehavior(new StartBehavior("none"), ct);
            await session.ExecuteBehavior(new OpenPageBehavior(url), ct);
            await session.ExecuteBehavior(new ScrollToBottomRetriable(5), ct);
            await session.ExecuteBehavior(new ScrollToTopRetriable(5), ct);
            await session.ExecuteBehavior(
                new GetNewElementRetriable(pool, path, type, name, 5),
                ct
            );
            await session.ExecuteBehavior(new StopBehavior(), ct);

            Result<WebElement> date = pool[^1];
            Assert.True(date.IsSuccess);
            string html = date.Value.OuterHTML;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var dateNode = doc.DocumentNode.FirstChild;
            var dateText = dateNode.LastChild;
            string text = dateText.InnerText;
            _logger.Information("{Date}", text);
        }
        catch (Exception ex)
        {
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
