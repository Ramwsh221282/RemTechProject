using Rabbit.RPC.Client.Abstractions;
using SharedParsersLibrary.Models;

namespace SharedParsersLibrary.Sinking;

public sealed class SinkerSender : IDisposable
{
    private readonly SingleCommunicationPublisher _publisher;

    public SinkerSender(SingleCommunicationPublisher publisher) => _publisher = publisher;

    public async Task Sink(ScrapedAdvertisement advertisement, string parserName)
    {
        SinkingAdvertisement toSink = new SinkingAdvertisement(advertisement, parserName);
        SinkAdvertisement sink = new SinkAdvertisement(toSink);
        await _publisher.Send(sink);
    }

    public void Dispose() => _publisher.Dispose();
}
