using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Parser;
using RemTechAvito.Infrastructure.Repository;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service;

namespace RemTechAvito.Integrational.Tests.BaseFilterParsingTests;

public sealed class BasicParseTransportTypeTest : BasicParserTests
{
    [Fact]
    public async Task Invoke_Basic()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        ITransportTypesParser parser = new TransportTypesParser(publisher, _logger);
        ITransportTypesRepository repository = new TransportTypesMOKRepository(_logger);
        ParseTransportTypesCommand command = new();
        ParseTransportTypesCommandHandler handler = new(parser, repository, _logger);

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        Result execution = await handler.Handle(command, ct);

        Assert.True(execution.IsSuccess);

        await worker.StopAsync(ct);
    }
}
