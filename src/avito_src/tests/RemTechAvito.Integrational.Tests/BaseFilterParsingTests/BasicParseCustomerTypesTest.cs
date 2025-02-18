using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.FiltersManagement.CustomerTypes.Commands.ParseCustomerTypes;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace RemTechAvito.Integrational.Tests.BaseFilterParsingTests;

public sealed class BasicParseCustomerTypesTest : BasicParserTests
{
    [Fact]
    public async Task Invoke_Customer_Types_Parse_UseCase()
    {
        bool noException = true;
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        try
        {
            ParseCustomerTypesCommand command = new();
            IAvitoCommandHandler<ParseCustomerTypesCommand> handler =
                _serviceProvider.GetRequiredService<
                    IAvitoCommandHandler<ParseCustomerTypesCommand>
                >();
            await handler.Handle(command, ct);
        }
        catch (Exception ex)
        {
            SingleCommunicationPublisher publisher = new SingleCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            await publisher.Send(new StopWebDriverContract(), ct);
            noException = false;
            _logger.Fatal(
                "{Test} failed. Exception: {ex}",
                nameof(Invoke_Customer_Types_Parse_UseCase),
                ex.Message
            );
        }
        await worker.StopAsync(ct);
        Assert.True(noException);
    }
}
