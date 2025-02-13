using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.FiltersManagement.CustomerStates.Commands.ParseCustomerStates;
using RemTechAvito.Application.FiltersManagement.CustomerTypes.Commands.ParseCustomerTypes;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Parser;
using RemTechAvito.Infrastructure.Repository;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service;

namespace RemTechAvito.Integrational.Tests.BaseFilterParsingTests;

public sealed class BasicParseCustomerStatesTest : BasicParserTests
{
    [Fact]
    public async Task Invoke_Base_Parse_Customer_States()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        ICustomerStatesParser parser = new CustomerStatesParser(publisher, _logger);
        ICustomerStatesRepository repository = new CustomerStatesMOKRepository(_logger);
        ParseCustomerStatesCommand command = new();
        ParseCustomerStatesCommandHandler handler = new(parser, repository, _logger);
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        Result execution = await handler.Handle(command, ct);

        Assert.True(execution.IsSuccess);

        await worker.StopAsync(ct);
    }
}
