using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing;

internal sealed class ParseAddressBehavior : IWebDriverBehavior
{
    private const string pathType = "xpath";
    private const string addressPath = ".//div[@itemprop='address']";
    private const string address = "address";
    private const string addressConcreteNodePath =
        ".//span[@class='style-item-address__string-wt61A']";
    private readonly CatalogueItem _item;
    private readonly ILogger _logger;

    public ParseAddressBehavior(CatalogueItem item, ILogger logger)
    {
        _item = item;
        _logger = logger;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        WebElementPool pool = new WebElementPool();
        GetNewElementRetriable getAddress = new GetNewElementRetriable(
            pool,
            addressPath,
            pathType,
            address,
            5
        );
        ClearPoolBehavior clear = new ClearPoolBehavior();

        await getAddress.Execute(publisher, ct);
        await clear.Execute(publisher, ct);

        InitializeAddress(pool);
        return Result.Success();
    }

    private void InitializeAddress(WebElementPool pool)
    {
        try
        {
            Result<WebElement> element = pool[^1];
            if (element.IsFailure)
            {
                _logger.Error("{Action} address was not found.", nameof(ParseAddressBehavior));
                return;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(element.Value.OuterHTML);
            HtmlNode? addressNode = doc.DocumentNode.SelectSingleNode(addressConcreteNodePath);
            if (addressNode == null)
            {
                _logger.Error("{Action} address node was not found.", nameof(ParseAddressBehavior));
                return;
            }

            _item.Address = addressNode.InnerText;
            _logger.Information(
                "{Action}. address: {Text}",
                nameof(ParseAddressBehavior),
                _item.Address
            );
        }
        catch (Exception ex)
        {
            _logger.Fatal("{Action} {Exception}", nameof(ParseAddressBehavior), ex.Message);
        }
    }
}
