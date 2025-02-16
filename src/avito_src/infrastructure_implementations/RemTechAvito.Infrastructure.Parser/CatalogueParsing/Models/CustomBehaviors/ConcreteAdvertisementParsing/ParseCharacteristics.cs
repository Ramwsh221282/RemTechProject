using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing;

internal sealed class ParseCharacteristics(CatalogueItem item, ILogger logger) : IWebDriverBehavior
{
    private const string pathType = "xpath";
    private const string containerPath = ".//ul[@class='params-paramsList-_awNW']";
    private const string container = "characteristics-container";
    private const string characteristicPath = ".//li[@class='params-paramsList__item-_2Y2O']";

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        WebElementPool pool = new WebElementPool();
        GetNewElementRetriable getContainer = new GetNewElementRetriable(
            pool,
            containerPath,
            pathType,
            container,
            5
        );
        ClearPoolBehavior clear = new ClearPoolBehavior();

        await getContainer.Execute(publisher, ct);
        await clear.Execute(publisher, ct);
        InitializeCharacteristics(pool);
        return Result.Success();
    }

    private void InitializeCharacteristics(WebElementPool pool)
    {
        try
        {
            Result<WebElement> element = pool[^1];
            if (element.IsFailure)
            {
                logger.Error(
                    "{Action} cannot get characteristics container.",
                    nameof(ParseCharacteristics)
                );
                return;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(element.Value.OuterHTML);
            HtmlNodeCollection? characteristicsNode = doc.DocumentNode.SelectNodes(
                characteristicPath
            );
            if (characteristicsNode == null)
            {
                logger.Error(
                    "{Action} cannot get characteristics collection nodes.",
                    nameof(ParseCharacteristics)
                );
                return;
            }

            if (characteristicsNode.Count == 0)
            {
                logger.Error(
                    "{Action} characteristics node count is 0.",
                    nameof(ParseCharacteristics)
                );
                return;
            }

            item.Characteristics = new string[characteristicsNode.Count];
            int lastInitializationIndex = 0;
            foreach (var node in characteristicsNode)
            {
                if (node == null)
                {
                    item.Characteristics[lastInitializationIndex] = String.Empty;
                    lastInitializationIndex++;
                    continue;
                }

                item.Characteristics[lastInitializationIndex] = node.InnerText;
                lastInitializationIndex++;
            }
        }
        catch (Exception ex)
        {
            logger.Fatal("{Action} {Exception}", nameof(ParseCharacteristics), ex.Message);
        }
    }
}
