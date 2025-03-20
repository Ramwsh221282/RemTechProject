using Rabbit.RPC.Client.Abstractions;
using SharedParsersLibrary.Configuration;

namespace SharedParsersLibrary.Sinking;

public sealed class SinkerSenderFactory
{
    private readonly ScrapedAdvertisementsSinkerConfiguration _configuration;

    public SinkerSenderFactory(ScrapedAdvertisementsSinkerConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SinkerSender CreateSinker() =>
        new SinkerSender(
            new SingleCommunicationPublisher(
                _configuration.QueueName,
                _configuration.HostName,
                _configuration.Username,
                _configuration.Password
            )
        );
}
