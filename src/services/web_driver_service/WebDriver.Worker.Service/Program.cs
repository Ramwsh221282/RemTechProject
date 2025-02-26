using WebDriver.Worker.Service;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.InitializeWorkerDependencies(
    "web-driver-service",
    "localhost",
    "rmuser",
    "rmpassword"
);
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
