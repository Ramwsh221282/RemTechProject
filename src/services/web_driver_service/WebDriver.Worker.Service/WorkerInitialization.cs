﻿using Rabbit.RPC.Server.Abstractions.Core;
using WebDriver.Application.Injection;
using WebDriver.Worker.Service.Contracts.ClearPool;
using WebDriver.Worker.Service.Contracts.ClearText;
using WebDriver.Worker.Service.Contracts.ClickOnElement;
using WebDriver.Worker.Service.Contracts.GetElementAttributeValue;
using WebDriver.Worker.Service.Contracts.GetElementHtml;
using WebDriver.Worker.Service.Contracts.GetMultipleChildren;
using WebDriver.Worker.Service.Contracts.GetPageHtml;
using WebDriver.Worker.Service.Contracts.GetSingleChildElement;
using WebDriver.Worker.Service.Contracts.GetSingleElement;
using WebDriver.Worker.Service.Contracts.GetTextFromElement;
using WebDriver.Worker.Service.Contracts.OpenWebDriverPage;
using WebDriver.Worker.Service.Contracts.ScrollElement;
using WebDriver.Worker.Service.Contracts.ScrollPageDown;
using WebDriver.Worker.Service.Contracts.ScrollPageTop;
using WebDriver.Worker.Service.Contracts.SendTextOnElement;
using WebDriver.Worker.Service.Contracts.SendTextOnElementNoClear;
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
        registration.RegisterContract<ClickOnElementContract, ClickOnElementContractHandler>();
        registration.RegisterContract<GetPageHtmlContract, GetPageHtmlContractHandler>();
        registration.RegisterContract<GetElementHtmlContract, GetElementHtmlContractHandler>();
        registration.RegisterContract<
            SendTextOnElementContract,
            SendTextOnElementContractHandler
        >();
        registration.RegisterContract<
            GetElementAttributeValueContract,
            GetElementAttributeValueContractHandler
        >();
        registration.RegisterContract<ScrollElementContract, ScrollElementContractHandler>();
        registration.RegisterContract<
            SendTextOnElementNoClearContract,
            SendTextOnElementNoClearContractHandler
        >();
        registration.RegisterContract<ClearTextContract, ClearTextContractHandler>();
        registration.RegisterContract<ClearPoolContract, ClearPoolContractHandler>();

        registration.RegisterConnectionFactory(
            new SimpleConnectionFactory(hostname, username, password)
        );

        services.RegisterWebDriverServices();
        services = registration.RegisterServer(services, queueName);
    }
}
