using RemTech.Infrastructure;
using RemTech.Infrastructure.PostgreSql;
using RemTech.Parser.Drom;
using Serilog;
using SharedParsersLibrary.DatabaseSinking;
using SharedParsersLibrary.DependencyInjection;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Services.AddSingleton(logger);
builder.Services.InjectConnectionString(Constants.PostgreSqlFilePath);
builder.Services.InjectParserDependencies();

builder.Services.AddHostedService<Worker>(p =>
{
    DatabaseSinkingFacade sinking = p.GetRequiredService<DatabaseSinkingFacade>();
    ParserManagementFacade parser = p.GetRequiredService<ParserManagementFacade>();
    return new Worker(logger, sinking, parser, "DROM");
});

IHost host = builder.Build();
host.Run();
