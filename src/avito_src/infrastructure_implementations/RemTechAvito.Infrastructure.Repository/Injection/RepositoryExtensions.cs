using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.CustomerStatesFilterManagement;
using RemTechAvito.Infrastructure.Repository.CustomerTypesFilterManagement;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechAvito.Infrastructure.Repository.TransportStatesFilterManagement;
using RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement;

namespace RemTechAvito.Infrastructure.Repository.Injection;

public static class RepositoryExtensions
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        TransportAdvertisementsRepository.RegisterSerializers();
        TransportAdvertisementsRepository.RegisterBsonClassMap();
        services.AddSingleton<MongoDbOptions>();
        services.AddSingleton<MongoClient>(p =>
        {
            MongoDbOptions options = p.GetRequiredService<MongoDbOptions>();
            MongoClient client = new MongoClient(options.ConnectionString);
            TransportAdvertisementsRepository.RegisterIndexes(client).Wait();
            return client;
        });
        services.AddScoped<
            ITransportAdvertisementsCommandRepository,
            TransportAdvertisementsCommandRepository
        >();
        services.AddScoped<
            ITransportAdvertisementsQueryRepository,
            TransportAdvertisementsQueryRepository
        >();
        services.AddScoped<ITransportStatesRepository, TransportStatesRepository>();
        services.AddScoped<ITransportTypesRepository, TransportTypesRepository>();
        services.AddScoped<ICustomerStatesRepository, CustomerStatesRepository>();
        services.AddScoped<ICustomerTypesRepository, CustomerTypesRepository>();
        return services;
    }
}
