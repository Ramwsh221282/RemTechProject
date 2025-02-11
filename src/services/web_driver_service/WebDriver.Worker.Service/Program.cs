using WebDriver.Application;
using WebDriver.Worker.Service;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.InitializeWorkerDependencies(
    "web-driver-service",
    "localhost",
    "rmuser",
    "rmpassword"
);

var provider = builder.Services.BuildServiceProvider();
WebDriverApi api = provider.GetRequiredService<WebDriverApi>();

builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
