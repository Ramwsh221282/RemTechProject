using WebDriver.Worker.Service;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.InitializeWorkerDependencies(
    "web-driver-service",
    "localhost",
    "rmuser",
    "rmpassword"
);

var host = builder.Build();
host.Run();
