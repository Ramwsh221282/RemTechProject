using AvitoParserService;
using Serilog;
using SharedParsersLibrary.DependencyInjection;
using ILogger = Serilog.ILogger;

var builder = Host.CreateApplicationBuilder(args);
ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Services.AddSingleton(logger);
builder.Services.InjectParserDependencies();
builder.Services.AddScoped<Worker>();
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
