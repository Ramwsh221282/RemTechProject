using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing.PhoneNumberParsing;

internal sealed class PhoneNumberParser : IWebDriverBehavior
{
    private readonly CatalogueItem _item;
    private readonly ILogger _logger;
    private const string UrlPrefix = "https://m.avito.ru/api/1/items/";
    private const string UrlSuffix = "/phone?key=af0deccbgcgidddjgnvljitntccdduijhdinfgjgfjir";
    private const string name = "json";
    private const string path = ".//pre";
    private const string type = "xpath";

    public PhoneNumberParser(CatalogueItem item, ILogger logger)
    {
        _item = item;
        _logger = logger;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_item.Id))
                return Result.Success();

            WebElementPool pool = new WebElementPool();
            OpenPageBehavior open = new OpenPageBehavior(BuildUrl());
            GetNewElementRetriable getPre = new GetNewElementRetriable(pool, path, type, name, 10);
            ClearPoolBehavior clear = new ClearPoolBehavior();

            await open.Execute(publisher, ct);
            await getPre.Execute(publisher, ct);
            await clear.Execute(publisher, ct);
            await Task.Delay(TimeSpan.FromSeconds(5), ct); // IMPORTANT!

            ExtractPhoneNumber(pool);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "{Action} unable to parse phone. Exception: {Ex}",
                nameof(PhoneNumberParsing),
                ex.Message
            );
            return new Error("Unable to parse mobile phone.");
        }
    }

    private string BuildUrl() => string.Concat(UrlPrefix, _item.Id, UrlSuffix);

    private void ExtractPhoneNumber(WebElementPool pool)
    {
        Result<WebElement> element = pool[^1];
        if (element.IsFailure)
            return;

        ReadOnlySpan<char> textSpan = element.Value.InnerText;
        int indexOfPhoneStart = textSpan.IndexOf('+');
        if (indexOfPhoneStart == -1)
            return;

        int sliceLength = 0;
        for (int index = indexOfPhoneStart + 1; ; index++)
        {
            if (textSpan[index] == '"')
                break;
            sliceLength++;
        }

        ReadOnlySpan<char> phoneSpan = textSpan.Slice(indexOfPhoneStart + 1, sliceLength);
        _item.SellerInfo.SellerContacts = $"{phoneSpan}";
    }
}
