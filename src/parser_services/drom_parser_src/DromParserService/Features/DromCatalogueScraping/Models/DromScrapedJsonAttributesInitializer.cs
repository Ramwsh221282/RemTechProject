using System.Text.Json;
using RemTechCommon.Utils.ResultPattern;
using SharedParsersLibrary.Models;

namespace DromParserService.Features.DromCatalogueScraping.Models;

public sealed class DromScrapedJsonAttributesInitializer : IDromScrapedAdvertisementProperty
{
    private readonly Result<AdditionalCandyConfig> _configResult;
    private readonly Result<BullCounters> _bullCountersResult;
    private readonly DromTransportAttributeType[] _attributeTypeResults;
    private readonly Result<GeoInfo> _geoInfoResult;
    private readonly Result<AdditionalInfo> _additionalInfoResult;

    public DromScrapedJsonAttributesInitializer(JsonDocument document)
    {
        _configResult = AdditionalCandyConfigFactory.FromJsonDocument(document);
        _bullCountersResult = BullCountersFactory.FromJsonDocument(document);
        _attributeTypeResults = DromTransportAttributeTypeFactory.CreateFromJsonDocument(document);
        _geoInfoResult = GeoInfoFactory.FromJsonDocument(document);
        _additionalInfoResult = AdditionalInfoFactory.FromJsonDocument(document);
    }

    public ScrapedAdvertisement Set(ScrapedAdvertisement advertisement)
    {
        ScrapedAdvertisement clone = advertisement.Clone();
        if (_configResult.IsSuccess)
            clone = _configResult.Value.Set(clone);
        if (_bullCountersResult.IsSuccess)
            clone = _bullCountersResult.Value.Set(clone);
        if (_geoInfoResult.IsSuccess)
            clone = _geoInfoResult.Value.Set(clone);
        if (_additionalInfoResult.IsSuccess)
            clone = _additionalInfoResult.Value.Set(clone);
        clone = _attributeTypeResults.Aggregate(clone, (current, result) => result.Set(current));
        return clone;
    }
}
