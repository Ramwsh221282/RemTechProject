using DromParserService;
using Serilog;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);
Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Services.AddSingleton(logger);
builder.Services.AddHostedService<Worker>();
builder.Services.InjectParserDependencies();

IServiceProvider provider = builder.Services.BuildServiceProvider();
IScrapeAdvertisementsHandler handler = provider.GetRequiredService<IScrapeAdvertisementsHandler>();
ScrapeAdvertisementCommand command = new ScrapeAdvertisementCommand(
    "https://auto.drom.ru/spec/bull/loader/all/"
);
await handler.Handle(command);

var host = builder.Build();
host.Run();
