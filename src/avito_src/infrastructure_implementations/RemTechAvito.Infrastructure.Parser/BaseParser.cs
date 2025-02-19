using Rabbit.RPC.Client.Abstractions;
using Serilog;

namespace RemTechAvito.Infrastructure.Parser;

internal abstract class BaseParser
{
    protected const string Url =
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";
    protected readonly IMessagePublisher _publisher;
    protected readonly ILogger _logger;

    protected BaseParser(IMessagePublisher publisher, ILogger logger)
    {
        _publisher = publisher;
        _logger = logger;
    }
}
