using AvitoParser.PDK.Sinking.Models;
using Rabbit.RPC.Client.Abstractions;

namespace AvitoParser.PDK.Sink;

public record SinkScrapedAdvertisementsContract(
    ScrapedAdvertisementSink[] Advertisements,
    ScrapedFromSink From
) : IContract;
