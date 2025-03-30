using DromParserService;
using Serilog;
using SharedParsersLibrary.DependencyInjection;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Services.AddSingleton(logger);
builder.Services.InjectParserDependencies();
builder.Services.AddSingleton<Worker>();
builder.Services.AddHostedService<Worker>();
IHost host = builder.Build();
host.Run();
