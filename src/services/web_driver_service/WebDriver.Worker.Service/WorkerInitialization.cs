using Rabbit.RPC.Server.Abstractions.Core;
using WebDriver.Worker.Service.Contracts.OpenWebDriverPage;
using WebDriver.Worker.Service.Contracts.StartWebDriver;
using WebDriver.Worker.Service.Contracts.StopWebDriver;

namespace WebDriver.Worker.Service;

public static class WorkerInitialization
{
    public static void InitializeWorkerDependencies(
        this IServiceCollection services,
        string queueName,
        string hostname,
        string username,
        string password
    )
    {
        ServerRegistrationContext registration = new ServerRegistrationContext();
        registration.RegisterContract<StartWebDriverContract, StartWebDriverContractHandler>();
        registration.RegisterContract<
            OpenWebDriverPageContract,
            OpenWebDriverPageContractHandler
        >();
        registration.RegisterContract<StopWebDriverContract, StopWebDriverContractHandler>();
        registration.RegisterConnectionFactory(
            new SimpleConnectionFactory(hostname, username, password)
        );
        registration.RegisterServer(services, queueName);
    }
}
