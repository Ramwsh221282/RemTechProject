using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.CustomerStatesFilterManagement;
using RemTechAvito.Infrastructure.Repository.CustomerTypesFilterManagement;
using RemTechAvito.Infrastructure.Repository.Specifications;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;
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
        services.AddSingleton<
            ITransportAdvertisementsCommandRepository,
            TransportAdvertisementsCommandRepository
        >();
        services.AddSingleton<
            ITransportAdvertisementsQueryRepository,
            TransportAdvertisementsQueryRepository
        >();
        services.AddSingleton<ITransportStatesRepository, TransportStatesRepository>();
        services.AddSingleton<ITransportTypesRepository, TransportTypesRepository>();
        services.AddSingleton<ICustomerStatesRepository, CustomerStatesRepository>();
        services.AddSingleton<ICustomerTypesRepository, CustomerTypesRepository>();
        services.AddSingleton<TransportAdvertisementsQueryResolver>();
        services.RegisterTransportAdvertisementQueries();
        return services;
    }

    private static IServiceCollection RegisterTransportAdvertisementQueries(
        this IServiceCollection services
    )
    {
        services.AddSingleton<
            IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>,
            AdvertisementAddressFilterQuery
        >();
        services.AddSingleton<
            IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>,
            AdvertisementTextSearchQuery
        >();
        services.AddSingleton<
            IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>,
            AdvertisementPriceFilterQuery
        >();
        services.AddSingleton<
            IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>,
            AdvertisementCharacteristicsFilterQuery
        >();
        services.AddSingleton<
            IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>,
            AdvertisementTextSearchQuery
        >();
        services.AddSingleton<
            IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>,
            AdvertisementPriceRangeFilterQuery
        >();
        return services;
    }
}
