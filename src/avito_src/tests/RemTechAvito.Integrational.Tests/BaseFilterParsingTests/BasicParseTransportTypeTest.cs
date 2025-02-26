using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;
using RemTechAvito.Contracts.Common.Responses.TransportTypesManagement;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace RemTechAvito.Integrational.Tests.BaseFilterParsingTests;

public sealed class BasicParseTransportTypeTest : BasicParserTests
{
    [Fact]
    public async Task Invoke_Basic_Transport_Types_Parse_Use_Case()
    {
        var noException = true;
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;
        try
        {
            ParseTransportTypesCommand command = new();
            var handler = _serviceProvider.GetRequiredService<
                IAvitoCommandHandler<ParseTransportTypesCommand, TransportTypeResponse>
            >();
            await handler.Handle(command, ct);
        }
        catch (Exception ex)
        {
            noException = false;
            var stopper = new SingleCommunicationPublisher(queue, host, user, password);
            await stopper.Send(new StopWebDriverContract());
            _logger.Fatal("Test finished. Exception: {Ex}", ex.Message);
        }

        Assert.True(noException);
    }
}
