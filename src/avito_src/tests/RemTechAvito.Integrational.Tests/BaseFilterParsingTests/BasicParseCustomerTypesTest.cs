using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.FiltersManagement.CustomerTypes.Commands.ParseCustomerTypes;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Parser;
using RemTechAvito.Infrastructure.Repository;
using RemTechAvito.Infrastructure.Repository.CustomerTypesFilterManagement;
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
            CustomerTypesRepository.RegisterSerializer();
            MongoDbOptions options = new MongoDbOptions();
            options.ConnectionString = "mongodb://root:example@localhost:27017/?authSource=admin";
            MongoClient client = new MongoClient(options.ConnectionString);
            IMessagePublisher publisher = new MultiCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            ICustomerTypesParser parser = new CustomerTypesParser(publisher, _logger);
            ICustomerTypesRepository repository = new CustomerTypesRepository(client, _logger);
            ParseCustomerTypesCommand command = new();
            ParseCustomerTypesCommandHandler handler = new(parser, repository, _logger);
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
