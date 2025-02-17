using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing.PhoneNumberParsing;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing.PhotoParsing;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors;

internal sealed class AdvertisementParser
{
    private readonly CatalogueItem _item;
    private readonly ILogger _logger;
    private readonly OpenPageBehavior _open;
    private readonly ScrollToBottomRetriable _bottom;
    private readonly ScrollToTopRetriable _top;
    private readonly ParseSellerInfoBehavior _seller;
    private readonly ParseCharacteristics _characteristics;
    private readonly ParseDateBehavior _date;
    private readonly PhotoClickingBehavior _photos;
    private readonly PhoneNumberParser _phone;
    private readonly IMessagePublisher _publisher;

    public AdvertisementParser(CatalogueItem item, IMessagePublisher publisher, ILogger logger)
    {
        _item = item;
        _logger = logger;
        _publisher = publisher;
        _open = new OpenPageBehavior(item.Url);
        _bottom = new ScrollToBottomRetriable(5);
        _top = new ScrollToTopRetriable(5);
        _seller = new ParseSellerInfoBehavior(item, logger);
        _characteristics = new ParseCharacteristics(item, logger);
        _date = new ParseDateBehavior(item, logger);
        _photos = new PhotoClickingBehavior(item, logger);
        _phone = new PhoneNumberParser(item, logger);
    }

    public async Task Execute(CancellationToken ct = default)
    {
        try
        {
            await _open.Execute(_publisher, ct);
            await _bottom.Execute(_publisher, ct);
            await _top.Execute(_publisher, ct);
            await _seller.Execute(_publisher, ct);
            await _characteristics.Execute(_publisher, ct);
            await _date.Execute(_publisher, ct);
            await _photos.Execute(_publisher, ct);
            await _phone.Execute(_publisher, ct);

            _logger.Information(
                "{Action}. {Url} was parsed",
                nameof(AdvertisementParser),
                _item.Url
            );
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "{Action} {Url} was not parsed. Exception: {Ex}",
                nameof(AdvertisementParser),
                _item.Url,
                ex.Message
            );
        }
    }
}
