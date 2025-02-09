using Rabbit.RPC.Server.Abstractions.Core;
using WebDriver.Core.Injection;
using WebDriver.Worker.Service.Contracts.GetMultipleChildren;
using WebDriver.Worker.Service.Contracts.GetSingleChildElement;
using WebDriver.Worker.Service.Contracts.GetSingleElement;
using WebDriver.Worker.Service.Contracts.GetTextFromElement;
using WebDriver.Worker.Service.Contracts.OpenWebDriverPage;
using WebDriver.Worker.Service.Contracts.ScrollPageDown;
using WebDriver.Worker.Service.Contracts.ScrollPageTop;
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
        services.RegisterWebDriverServices();

        ServerRegistrationContext registration = new ServerRegistrationContext();

        registration.RegisterContract<StartWebDriverContract, StartWebDriverContractHandler>();
        registration.RegisterContract<
            OpenWebDriverPageContract,
            OpenWebDriverPageContractHandler
        >();
        registration.RegisterContract<StopWebDriverContract, StopWebDriverContractHandler>();
        registration.RegisterContract<GetSingleElementContract, GetSingleElementContractHandler>();
        registration.RegisterContract<ScrollPageDownContract, ScrollPageDownContractHandler>();
        registration.RegisterContract<ScrollPageTopContract, ScrollPageTopContractHandler>();
        registration.RegisterContract<
            GetSingleChildElementContract,
            GetSingleChildElementContractHandler
        >();
        registration.RegisterContract<
            GetMultipleChildrenContract,
            GetMultipleChildrenContractHandler
        >();
        registration.RegisterContract<
            GetTextFromElementContract,
            GetTextFromElementContractHandler
        >();

        registration.RegisterConnectionFactory(
            new SimpleConnectionFactory(hostname, username, password)
        );

        services = registration.RegisterServer(services, queueName);
    }
}
