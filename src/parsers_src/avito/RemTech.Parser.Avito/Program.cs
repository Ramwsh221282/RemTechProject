using RemTech.Infrastructure;
using RemTech.Infrastructure.PostgreSql;
using RemTech.Parser.Avito;
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
    DatabaseSinkingFacade dbFacade = p.GetRequiredService<DatabaseSinkingFacade>();
    ParserManagementFacade parserFacade = p.GetRequiredService<ParserManagementFacade>();
    return new Worker("AVITO", logger, dbFacade, parserFacade);
});
IHost host = builder.Build();
host.Run();
