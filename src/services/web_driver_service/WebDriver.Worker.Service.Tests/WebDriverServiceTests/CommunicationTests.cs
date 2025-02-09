using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.OpenWebDriverPage;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.StartWebDriver;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.StopWebDriver;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests;

public sealed class CommunicationTests
{
    private const string localhost = "localhost";
    private const string user = "rmuser";
    private const string password = "rmpassword";
    private const string queue = "web-driver-service";

    private readonly IServiceProvider _serviceProvider;

    public CommunicationTests()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddLogging();
        collection.InitializeWorkerDependencies(queue, localhost, user, password);
        collection.AddSingleton<Worker>();
        _serviceProvider = collection.BuildServiceProvider();
    }

    [Fact]
    public async Task Test_Serialization_And_Deserialization()
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

        ContractActionResult result_1 = await publisher.SendCommand(new StartWebDriverContract());
        Assert.True(result_1.IsSuccess);
        StartWebDriverContractResponse response_1 =
            result_1.FromResult<StartWebDriverContractResponse>();
        Assert.True(response_1.IsStarted);

        ContractActionResult result_2 = await publisher.SendCommand(
            new OpenWebDriverPageContract("vk.com")
        );
        Assert.True(result_2.IsSuccess);
        OpenWebDriverPageResponse response_2 = result_2.FromResult<OpenWebDriverPageResponse>();
        Assert.Equal("vk.com", response_2.OpenedUrl);

        ContractActionResult result_3 = await publisher.SendCommand(new StopWebDriverContract());
        Assert.True(result_3.IsSuccess);
        StopWebDriverContractResponse response =
            result_3.FromResult<StopWebDriverContractResponse>();
        Assert.True(response.IsStopped);

        await workerLife;
    }
}
