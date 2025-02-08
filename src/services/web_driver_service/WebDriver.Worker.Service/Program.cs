using Rabbit.RPC.Server.Abstractions.Core;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.OpenWebDriverPage;
using WebDriver.Worker.Service.Contracts.StartWebDriver;
using WebDriver.Worker.Service.Contracts.StopWebDriver;

var builder = Host.CreateApplicationBuilder(args);

ServerRegistrationContext registration = new ServerRegistrationContext();
registration.RegisterContract<StartWebDriverContract, StartWebDriverContractHandler>();
registration.RegisterContract<OpenWebDriverPageContract, OpenWebDriverPageContractHandler>();
registration.RegisterContract<StopWebDriverContract, StopWebDriverContractHandler>();
registration.RegisterConnectionFactory(
    new SimpleConnectionFactory("localhost", "rmuser", "rmpassword")
);
registration.RegisterServer(builder.Services, "web-driver-service");

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
