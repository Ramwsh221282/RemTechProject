using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.FiltersManagement.CustomerStates.Commands.ParseCustomerStates;
using RemTechAvito.Application.FiltersManagement.CustomerTypes.Commands.ParseCustomerTypes;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Parser;
using RemTechAvito.Infrastructure.Repository;
using RemTechAvito.Infrastructure.Repository.CustomerStatesFilterManagement;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace RemTechAvito.Integrational.Tests.BaseFilterParsingTests;

public sealed class BasicParseCustomerStatesTest : BasicParserTests
{
    [Fact]
    public async Task Invoke_Base_Parse_Customer_States()
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
            CustomerStatesRepository.RegisterSerializer();
            IMessagePublisher publisher = new MultiCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            ICustomerStatesParser parser = new CustomerStatesParser(publisher, _logger);
            ICustomerStatesRepository repository = new CustomerStatesRepository(client, _logger);
            ParseCustomerStatesCommand command = new();
            ParseCustomerStatesCommandHandler handler = new(parser, repository, _logger);
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
                "{Test} is failed. Exception: {Ex}",
                nameof(Invoke_Base_Parse_Customer_States),
                ex.Message
            );
        }
        await worker.StopAsync(ct);
        Assert.True(noExceptions);
    }
}
