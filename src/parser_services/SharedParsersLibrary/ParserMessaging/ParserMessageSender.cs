using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Configuration;
using SharedParsersLibrary.Models;

namespace SharedParsersLibrary.ParserMessaging;

public sealed class ParserMessageSender(ScrapedAdvertisementsSinkerConfiguration configuration)
{
    private readonly ScrapedAdvertisementsSinkerConfiguration _configuration = configuration;

    public async Task<Option<Parser>> RequestParser(string parserName)
    {
        using var publisher = CreatePublisher();
        ParserQueryPayload payload = new ParserQueryPayload(ServiceName: parserName);
        GetParserMessage message = new GetParserMessage(payload);
        ContractActionResult result = await publisher.Send(message);
        return result.IsSuccess
            ? Option<Parser>.Some(result.FromResult<ParserDaoResponse>().ToParser())
            : Option<Parser>.None();
    }

    public async Task<Option<Parser>> UpdateParser(Parser parser)
    {
        using var publisher = CreatePublisher();
        ParserDto dto = parser.ToParserDto();
        UpdateParserMessage message = new UpdateParserMessage(dto);
        ContractActionResult result = await publisher.Send(message);
        return result.IsSuccess ? Option<Parser>.Some(parser) : Option<Parser>.None();
    }

    private SingleCommunicationPublisher CreatePublisher() =>
        new SingleCommunicationPublisher(
            _configuration.QueueName,
            _configuration.HostName,
            _configuration.Username,
            _configuration.Password
        );
}
