using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Parser;
using RemTechAvito.Infrastructure.Repository;
using RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement;
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
            MongoDbOptions options = new MongoDbOptions();
            options.ConnectionString = "mongodb://root:example@localhost:27017/?authSource=admin";
            MongoClient client = new MongoClient(options.ConnectionString);
            IMessagePublisher publisher = new MultiCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            ITransportTypesParser parser = new TransportTypesParser(publisher, _logger);
            TransportTypesRepository.RegisterSerializers();
            ITransportTypesRepository repository = new TransportTypesRepository(client, _logger);
            ParseTransportTypesCommand command = new();
            ParseTransportTypesCommandHandler handler = new(parser, repository, _logger);
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
