using AvitoParser.PDK.Models;
using AvitoParser.PDK.Models.ValueObjects;
using AvitoParser.PDK.Sinking.Configurations;
using AvitoParser.PDK.Sinking.Models;
using AvitoParser.PDK.Sinking.RabbitMq;
using Microsoft.Extensions.DependencyInjection;

namespace AvitoParser.Tests.Avito_Parser_Plugins_Tests;

public sealed class SinkingTests
{
    private const string filePath = "SINK_CLIENT_CONFIG.json";
    private readonly RabbitSinker _sinker;

    public SinkingTests()
    {
        IServiceCollection services = new ServiceCollection();
        SinkingClientConfiguration configuration =
            SinkingClientConfiguration.SinkingClientConfigurationLoader.Load(filePath);
        services.AddSingleton(configuration);
        services.AddSingleton<RabbitSinker>();
        IServiceProvider provider = services.BuildServiceProvider();
        _sinker = provider.GetRequiredService<RabbitSinker>();
    }

    [Fact]
    public async Task Sink_Advertisement_Mok()
    {
        ScrapedAdvertisement advertisement = new ScrapedAdvertisement()
        {
            Address = ScrapedAddress.Create("test"),
            Characteristics = [ScrapedCharacteristic.Create("Test", "Test")],
            Date = ScrapedAdvertisementDate.Default,
            Description = ScrapedDescription.Create("test"),
            Id = ScrapedAdvertisementId.Create("123"),
            Photos = [ScrapedPhotoUrl.Create("http://localhost")],
            Price = ScrapedPrice.Create("123", "NDS"),
            Title = ScrapedTitle.Create("test"),
            Publisher = ScrapedPublisher.Create("test"),
            SourceUrl = ScrapedSourceUrl.Create("test"),
        };
        await _sinker.SinkAdvertisements([advertisement], new ScrapedFromSink("AVITO"));
    }
}
