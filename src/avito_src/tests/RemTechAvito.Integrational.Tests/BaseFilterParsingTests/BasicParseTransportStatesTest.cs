using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.FiltersManagement.TransportStates.Commands.ParseTransportStates;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Parser;
using RemTechAvito.Infrastructure.Repository;
using RemTechAvito.Infrastructure.Repository.TransportStatesFilterManagement;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace RemTechAvito.Integrational.Tests.BaseFilterParsingTests;

public sealed class BasicParseTransportStatesTest : BasicParserTests
{
    [Fact]
    public async Task Invoke_Basic_Transport_States_Test()
    {
        bool noExceptions = true;
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        try
        {
            MongoDbOptions options = new MongoDbOptions();
            options.ConnectionString = "mongodb://root:example@localhost:27017/?authSource=admin";
            MongoClient client = new MongoClient(options.ConnectionString);
            IMessagePublisher publisher = new MultiCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            ITransportStatesParser parser = new TransportStatesParser(publisher, _logger);
            TransportStatesRepository.RegisterSerializer();
            ITransportStatesRepository repository = new TransportStatesRepository(client, _logger);
            ParseTransportStatesCommand command = new();
            ParseTransportStatesCommandHandler handler = new(parser, repository, _logger);
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
            noExceptions = false;
            _logger.Fatal(
                "{Test} exception occured {Ex}",
                nameof(Invoke_Basic_Transport_States_Test),
                ex.Message
            );
        }

        await worker.StopAsync(ct);
        Assert.True(noExceptions);
    }
}
