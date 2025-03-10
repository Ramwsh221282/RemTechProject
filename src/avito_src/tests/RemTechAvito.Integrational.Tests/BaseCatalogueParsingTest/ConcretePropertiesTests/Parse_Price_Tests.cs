﻿using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Integrational.Tests.BaseCatalogueParsingTest.ConcretePropertiesTests;

public sealed class Parse_Price_Tests : BasicParserTests
{
    [Fact]
    public async Task Parse_Price_Sample_1()
    {
        const string url =
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_hifoune_fd30_2025_4373248650?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiJvSDRrWjg0SElwTlQwQnN2Ijt94UlT_z8AAAA";
        const string path = ".//div[@data-marker='item-view/item-price-container']";
        const string type = "xpath";
        const string name = "price";

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

            var parent = HtmlNode.CreateNode(html);
            var priceValueNode = parent.SelectSingleNode(
                ".//span[@data-marker='item-view/item-price']"
            );
            Assert.NotNull(priceValueNode);
            IEnumerable<HtmlAttribute> priceValueNodeAttributes = priceValueNode.GetAttributes();
            var priceValueAttribute = priceValueNodeAttributes.FirstOrDefault(a =>
                a.Name == "content"
            );
            Assert.NotNull(priceValueAttribute);
            var price = priceValueAttribute.Value;

            var currencyNode = parent.SelectSingleNode(".//span[@itemprop='priceCurrency']");
            Assert.NotNull(currencyNode);
            IEnumerable<HtmlAttribute> priceCurrencyAttributes = currencyNode.GetAttributes();
            var currencyAttribute = priceCurrencyAttributes.FirstOrDefault(a =>
                a.Name == "content"
            );
            Assert.NotNull(currencyAttribute);
            var currency = currencyAttribute.Value;

            var extraInfoNodeContainer = parent.SelectSingleNode(
                ".//span[@class='style-price-value-additional-pFInr']"
            );
            Assert.NotNull(extraInfoNodeContainer);
            var extra = extraInfoNodeContainer.InnerText;

            ReadOnlySpan<char> extraSpan = extra;
            var index = extraSpan.IndexOf(';');
            extraSpan = extraSpan.Slice(index + 1);
            extra = $"{extraSpan}";

            _logger.Information("Price value: {Price}", price);
            _logger.Information("Currency: {Currency}", currency);
            _logger.Information("Extras: {Extras}", extra);
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
