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
        var noException = true;
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        try
        {
            ParseCustomerTypesCommand command = new();
            var handler = _serviceProvider.GetRequiredService<
                IAvitoCommandHandler<ParseCustomerTypesCommand>
            >();
            await handler.Handle(command, ct);
        }
        catch (Exception ex)
        {
            var publisher = new SingleCommunicationPublisher(queue, host, user, password);
            await publisher.Send(new StopWebDriverContract(), ct);
            noException = false;
            _logger.Fatal(
                "{Test} failed. Exception: {ex}",
                nameof(Invoke_Customer_Types_Parse_UseCase),
                ex.Message
            );
        }

        Assert.True(noException);
    }
}
