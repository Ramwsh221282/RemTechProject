using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.FiltersManagement.TransportStates.Commands.ParseTransportStates;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace RemTechAvito.Integrational.Tests.BaseFilterParsingTests;

public sealed class BasicParseTransportStatesTest : BasicParserTests
{
    [Fact]
    public async Task Invoke_Basic_Transport_States_Test()
    {
        var noExceptions = true;
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        try
        {
            ParseTransportStatesCommand command = new();
            var handler = _serviceProvider.GetRequiredService<
                IAvitoCommandHandler<ParseTransportStatesCommand>
            >();
            await handler.Handle(command, ct);
        }
        catch (Exception ex)
        {
            var publisher = new SingleCommunicationPublisher(queue, host, user, password);
            await publisher.Send(new StopWebDriverContract(), ct);
            noExceptions = false;
            _logger.Fatal(
                "{Test} exception occured {Ex}",
                nameof(Invoke_Basic_Transport_States_Test),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }
}
