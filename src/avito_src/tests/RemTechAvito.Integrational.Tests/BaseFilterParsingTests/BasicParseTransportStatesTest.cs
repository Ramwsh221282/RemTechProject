using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.FiltersManagement.TransportStates.Commands.ParseTransportStates;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Parser;
using RemTechAvito.Infrastructure.Repository;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service;

namespace RemTechAvito.Integrational.Tests.BaseFilterParsingTests;

public sealed class BasicParseTransportStatesTest : BasicParserTests
{
    [Fact]
    public async Task Invoke_Basic_Transport_States_Test()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        ITransportStatesParser parser = new TransportStatesParser(publisher, _logger);
        ITransportStatesRepository repository = new TransportStatesMOKRepository(_logger);
        ParseTransportStatesCommand command = new();
        ParseTransportStatesCommandHandler handler = new(parser, repository, _logger);

        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);
        Result execution = await handler.Handle(command, ct);
        Assert.True(execution.IsSuccess);
        await worker.StopAsync(ct);
    }
}
