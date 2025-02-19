using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace RemTechAvito.Integrational.Tests.BaseFilterParsingTests;

public sealed class BasicParseTransportTypeTest : BasicParserTests
{
    [Fact]
    public async Task Invoke_Basic_Transport_Types_Parse_Use_Case()
    {
        bool noException = true;
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);
        try
        {
            ParseTransportTypesCommand command = new();
            IAvitoCommandHandler<ParseTransportTypesCommand> handler =
                _serviceProvider.GetRequiredService<
                    IAvitoCommandHandler<ParseTransportTypesCommand>
                >();
            await handler.Handle(command, ct);
        }
        catch (Exception ex)
        {
            noException = false;
            SingleCommunicationPublisher stopper = new SingleCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            await stopper.Send(new StopWebDriverContract());
            _logger.Fatal("Test finished. Exception: {Ex}", ex.Message);
        }
        await worker.StopAsync(ct);
        Assert.True(noException);
    }
}
