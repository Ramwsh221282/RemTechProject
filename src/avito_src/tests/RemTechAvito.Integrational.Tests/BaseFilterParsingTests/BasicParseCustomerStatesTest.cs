using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.FiltersManagement.CustomerStates.Commands.ParseCustomerStates;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace RemTechAvito.Integrational.Tests.BaseFilterParsingTests;

public sealed class BasicParseCustomerStatesTest : BasicParserTests
{
    [Fact]
    public async Task Invoke_Base_Parse_Customer_States()
    {
        var noExceptions = true;
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        try
        {
            ParseCustomerStatesCommand command = new();
            var handler = _serviceProvider.GetRequiredService<
                IAvitoCommandHandler<ParseCustomerStatesCommand>
            >();
            await handler.Handle(command, ct);
        }
        catch (Exception ex)
        {
            var publisher = new SingleCommunicationPublisher(queue, host, user, password);
            await publisher.Send(new StopWebDriverContract(), ct);
            noExceptions = false;
            _logger.Fatal(
                "{Test} is failed. Exception: {Ex}",
                nameof(Invoke_Base_Parse_Customer_States),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }
}
