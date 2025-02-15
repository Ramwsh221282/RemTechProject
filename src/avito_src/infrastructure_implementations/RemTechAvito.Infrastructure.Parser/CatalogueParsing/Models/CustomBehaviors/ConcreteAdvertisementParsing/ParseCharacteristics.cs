using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing;

internal sealed class ParseCharacteristics : IWebDriverBehavior
{
    private const string pathType = "xpath";
    private const string containerPath = ".//ul[@class='params-paramsList-_awNW']";
    private const string container = "characteristics-container";

    private const string characteristicPath = ".//li[@class='params-paramsList__item-_2Y2O']";
    private const string characteristic = "characteristic";

    private readonly CatalogueItem _item;
    private readonly ILogger _logger;

    public ParseCharacteristics(CatalogueItem item, ILogger logger)
    {
        _item = item;
        _logger = logger;
    }

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
                _logger.Error(
                    "{Action} cannot get characteristics container.",
                    nameof(ParseCharacteristics)
                );
                return;
            }

            HtmlNode node = HtmlNode.CreateNode(element.Value.Model.ElementOuterHTML);
            HtmlNodeCollection? nodes = node.SelectNodes(characteristicPath);
            if (nodes is null)
            {
                _logger.Error(
                    "{Action} cannot get characteristics collection nodes.",
                    nameof(ParseCharacteristics)
                );
                return;
            }

            if (nodes.Count == 0)
            {
                _logger.Error(
                    "{Action} collection nodes count is 0.",
                    nameof(ParseCharacteristics)
                );
                return;
            }

            List<string> characteristics = [];
            foreach (var item in nodes)
            {
                if (item == null)
                {
                    _logger.Error(
                        "{Action} characteristics node is null",
                        nameof(ParseCharacteristics)
                    );
                    continue;
                }

                characteristics.Add(item.InnerText);
                _logger.Information(
                    "{Action} characteristics: {Text}",
                    nameof(ParseCharacteristics),
                    item.InnerText
                );
            }

            _item.Characteristics = characteristics.ToArray();
        }
        catch (Exception ex)
        {
            _logger.Fatal("{Action} {Exception}", nameof(ParseTitleBehavior), ex.Message);
        }
    }
}
