using Rabbit.RPC.Client.Abstractions;

namespace AvitoParser.PDK.Sinking.Models;

public record SinkScrapedAdvertisementsContract(
    ScrapedAdvertisementSink[] Advertisements,
    ScrapedFromSink From
) : IContract;
