using RemTech.MongoDb.Service;
using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement.Converters;
using RemTech.MongoDb.Service.Common.Models.ParsersManagement;
using RemTech.MongoDb.Service.Configurations;
using RemTech.MongoDb.Service.Configurations.MongoDbConfiguration;
using RemTech.MongoDb.Service.Configurations.RabbitMqListenerConfiguration;
using RemTech.MongoDb.Service.Features.AdvertisementsManagement.AdvertisementQuerying;
using RemTech.MongoDb.Service.Features.AdvertisementsSinking;
using RemTech.MongoDb.Service.Features.ParserManagement.ParserQuerying;
using RemTechCommon.Utils.DependencyInjectionHelpers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTransient<AdvertisementsFactory>();
builder.Services.AddTransient<SinkingAdvertisementValidator>();
builder.Services.AddMongoDb(ConfigurationVariables.MongoDbConfigFilePath);
builder.Services.AddRabbitMqConfigurationOptions(
    ConfigurationVariables.RabbitMqListenerConfigFilePath
);
builder.Services.AddSingleton<AdvertisementsRepository>();
builder.Services.AddSingleton<ParserRepository>();
builder.Services.AddScoped<IQueryBuilder<ParserQueryPayload, Parser>, ParserQueryBuilder>();
builder.Services.AddScoped<
    IQueryBuilder<AdvertisementQueryPayload, Advertisement>,
    AdvertisementQueryBuilder
>();
builder.Services.RegisterServices();
builder.Services.RegisterRabbitMqService();
builder.Services.AddSingleton<Worker>();
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
