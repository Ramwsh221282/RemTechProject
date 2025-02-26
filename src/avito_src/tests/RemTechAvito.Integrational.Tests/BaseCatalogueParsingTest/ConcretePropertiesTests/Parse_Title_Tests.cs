using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Integrational.Tests.BaseCatalogueParsingTest.ConcretePropertiesTests;

public sealed class Parse_Title_Tests : BasicParserTests
{
    [Fact]
    public async Task Test_Title_Sample_1()
    {
        const string url =
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_hifoune_fd30_2025_4373248650?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiJvSDRrWjg0SElwTlQwQnN2Ijt94UlT_z8AAAA";
        const string type = "xpath";
        const string path = ".//h1[@data-marker='item-view/title-info']";
        const string name = "title";

        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        try
        {
            IMessagePublisher publisher = new MultiCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            var pool = new WebElementPool();
            using var session = new WebDriverSession(publisher);
            await session.ExecuteBehavior(new StartBehavior("none"), ct);
            await session.ExecuteBehavior(new OpenPageBehavior(url), ct);
            await session.ExecuteBehavior(new ScrollToBottomRetriable(5), ct);
            await session.ExecuteBehavior(new ScrollToTopRetriable(5), ct);
            await session.ExecuteBehavior(
                new GetNewElementRetriable(pool, path, type, name, 5),
                ct
            );
            await session.ExecuteBehavior(new StopBehavior(), ct);

            var title = pool[^1];
            Assert.True(title.IsSuccess);
            Assert.NotEqual(string.Empty, title.Value.InnerText);
            _logger.Information("Title: {Title}", title.Value.InnerText);
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "Test running finished FATAL. Message: {Exception}. Source: {Source}",
                ex.Message,
                ex.Source
            );
        }
    }

    [Fact]
    public async Task Test_Title_Sample_2()
    {
        const string url =
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_hifoune_fd30_2025_4373248650?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiJvSDRrWjg0SElwTlQwQnN2Ijt94UlT_z8AAAA";
        const string type = "xpath";
        const string path = ".//h1[@data-marker='item-view/title-info']";
        const string name = "title";

        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        try
        {
            IMessagePublisher publisher = new MultiCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            var pool = new WebElementPool();
            using var session = new WebDriverSession(publisher);
            await session.ExecuteBehavior(new StartBehavior("none"), ct);
            await session.ExecuteBehavior(new OpenPageBehavior(url), ct);
            await session.ExecuteBehavior(new ScrollToBottomRetriable(5), ct);
            await session.ExecuteBehavior(new ScrollToTopRetriable(5), ct);
            await session.ExecuteBehavior(
                new GetNewElementRetriable(pool, path, type, name, 5),
                ct
            );
            await session.ExecuteBehavior(new StopBehavior(), ct);

            var title = pool[^1];
            Assert.True(title.IsSuccess);
            var html = title.Value.OuterHTML;
            Assert.NotEqual(string.Empty, html);
            var node = HtmlNode.CreateNode(html);
            var directInnerText = node.GetDirectInnerText();
            var innerText = node.InnerText;
            _logger.Information("Title (direct inner text): {Title}", directInnerText);
            _logger.Information("Title (just inner text: {Title}", innerText);
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "Test running finished FATAL. Message: {Exception}. Source: {Source}",
                ex.Message,
                ex.Source
            );
        }
    }
}
