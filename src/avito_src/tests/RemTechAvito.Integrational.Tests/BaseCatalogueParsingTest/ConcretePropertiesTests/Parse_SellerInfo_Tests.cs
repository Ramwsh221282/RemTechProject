using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Integrational.Tests.BaseCatalogueParsingTest.ConcretePropertiesTests;

public sealed class Parse_SellerInfo_Tests : BasicParserTests
{
    [Fact]
    public async Task Parse_SellerInfo_Sample_Company()
    {
        const string url =
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_hifoune_fd30_2025_4373248650?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiJvSDRrWjg0SElwTlQwQnN2Ijt94UlT_z8AAAA";
        const string type = "xpath";
        const string path = ".//div[@data-marker='item-view/seller-info']";
        const string name = "seller-info";

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
            var sellerNameNode = node.SelectSingleNode(".//div[@data-marker='seller-info/name']");
            var valuesContainer = sellerNameNode.FirstChild;
            var valuesWrapper = valuesContainer.FirstChild;
            var sellerName = valuesWrapper.FirstChild.InnerText;
            var sellerStatus = valuesWrapper.LastChild.InnerText;

            _logger.Information("Seller name: {Name}", sellerName);
            _logger.Information("Seller status: {Status}", sellerStatus);
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
    public async Task Parse_SellerInfo_Sample_Person()
    {
        const string url =
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_hifoune_fd30_2025_4373248650?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiJvSDRrWjg0SElwTlQwQnN2Ijt94UlT_z8AAAA";
        const string type = "xpath";
        const string path = ".//div[@data-marker='item-view/seller-info']";
        const string name = "seller-info";

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
            var sellerNameNode = node.SelectSingleNode(".//div[@data-marker='seller-info/name']");
            var valuesContainer = sellerNameNode.FirstChild;
            var valuesWrapper = valuesContainer.FirstChild;
            var sellerName = valuesWrapper.FirstChild.InnerText;
            var sellerStatus = valuesWrapper.LastChild.InnerText;

            _logger.Information("Seller name: {Name}", sellerName);
            _logger.Information("Seller status: {Status}", sellerStatus);
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
