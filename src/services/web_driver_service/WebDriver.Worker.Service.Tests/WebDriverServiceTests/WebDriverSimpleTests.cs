using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using Rabbit.RPC.Server.Abstractions.Core;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.OpenWebDriverPage;
using WebDriver.Worker.Service.Contracts.StartWebDriver;
using WebDriver.Worker.Service.Contracts.StopWebDriver;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests;

public sealed class WebDriverSimpleTests
{
    private const string localhost = "localhost";
    private const string user = "rmuser";
    private const string password = "rmpassword";
    private const string queue = "web-driver-service";

    private readonly IServiceProvider _serviceProvider;

    public WebDriverSimpleTests()
    {
        IServiceCollection collection = new ServiceCollection();
        ServerRegistrationContext registration = new ServerRegistrationContext();
        collection.AddLogging();
        registration.RegisterContract<StartWebDriverContract, StartWebDriverContractHandler>();
        registration.RegisterContract<
            OpenWebDriverPageContract,
            OpenWebDriverPageContractHandler
        >();
        registration.RegisterContract<StopWebDriverContract, StopWebDriverContractHandler>();
        registration.RegisterConnectionFactory(
            new SimpleConnectionFactory("localhost", "rmuser", "rmpassword")
        );
        registration.RegisterServer(collection, "web-driver-service");
        collection.AddSingleton<Worker>();
        _serviceProvider = collection.BuildServiceProvider();
    }

    [Fact]
    public async Task Test_Start_OpenPage_Stop_WebDriver()
    {
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task workerLife = worker.StartAsync(cancellationTokenSource.Token);

        SingleCommunicationPublisher publisher = new SingleCommunicationPublisher(
            queue,
            localhost,
            user,
            password
        );

        TestContracts.StartWebDriver.StartWebDriverContract contract_1 = new();
        Result? response_1 = await publisher.SendCommand<
            TestContracts.StartWebDriver.StartWebDriverContract,
            Result
        >(contract_1);
        await workerLife;
    }
}
