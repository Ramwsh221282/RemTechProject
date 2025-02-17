using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.FiltersManagement.CustomerTypes.Commands.ParseCustomerTypes;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Parser;
using RemTechAvito.Infrastructure.Repository;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service;

namespace RemTechAvito.Integrational.Tests.BaseFilterParsingTests;

public sealed class BasicParseCustomerTypesTest : BasicParserTests
{
    [Fact]
    public async Task Invoke_Customer_Types_Parse_UseCase()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        ICustomerTypesParser parser = new CustomerTypesParser(publisher, _logger);
        ICustomerTypesRepository repository = new CustomerTypesMOKRepository(_logger);
        ParseCustomerTypesCommand command = new();
        ParseCustomerTypesCommandHandler handler = new(parser, repository, _logger);

        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);
        Result execution = await handler.Handle(command, ct);
        Assert.True(execution.IsSuccess);
        await worker.StopAsync(ct);
    }
}
