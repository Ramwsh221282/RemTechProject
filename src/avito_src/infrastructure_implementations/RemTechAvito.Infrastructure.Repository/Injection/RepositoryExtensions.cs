using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.CustomerStatesFilterManagement;
using RemTechAvito.Infrastructure.Repository.CustomerTypesFilterManagement;
using RemTechAvito.Infrastructure.Repository.ParserJournalsManagement;
using RemTechAvito.Infrastructure.Repository.ParserProfileManagement;
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
        ParserProfileMetadata.RegisterMetadata();
        TransportTypesMetadata.RegisterMetadata();
        ParserJournalMetadata.RegisterMetadata();
        services.AddSingleton<MongoDbOptions>();
        services.AddSingleton<MongoClient>(p =>
        {
            var options = p.GetRequiredService<MongoDbOptions>();
            var client = new MongoClient(options.ConnectionString);
            TransportAdvertisementsRepository.RegisterIndexes(client).Wait();
            TransportTypesMetadata.RegisterIndexes(client).Wait();
            return client;
        });
        services.AddScoped<IParserJournalCommandRepository, ParserJournalCommandRepository>();
        services.AddScoped<IParserJournalQueryRepository, ParserJournalQueryRepository>();
        services.AddScoped<
            ITransportAdvertisementsCommandRepository,
            TransportAdvertisementsCommandRepository
        >();
        services.AddScoped<
            ITransportAdvertisementsQueryRepository,
            TransportAdvertisementsQueryRepository
        >();
        services.AddScoped<ITransportStatesRepository, TransportStatesRepository>();
        services.AddScoped<
            ITransportTypesCommandRepository,
            TransportTypesCommandCommandRepository
        >();
        services.AddScoped<ITransportTypesQueryRepository, TransportTypesQueryRepository>();
        services.AddScoped<ICustomerStatesRepository, CustomerStatesRepository>();
        services.AddScoped<ICustomerTypesRepository, CustomerTypesRepository>();
        services.AddScoped<IParserProfileCommandRepository, ParserProfileCommandRepository>();
        services.AddScoped<TransportAdvertisementsQueryResolver>();
        services.AddScoped<IParserProfileReadRepository, ParserProfileReadRepository>();
        services.RegisterTransportAdvertisementQueries();
        return services;
    }

    private static void RegisterTransportAdvertisementQueries(this IServiceCollection services)
    {
        services.AddSingleton<
            IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>,
            AdvertisementAddressFilterQuery
        >();
        services.AddSingleton<
            IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>,
            AdvertisementCharacteristicsFilterQuery
        >();
        services.AddSingleton<
            IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>,
            AdvertisementPriceFilterQuery
        >();
        services.AddSingleton<
            IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>,
            AdvertisementPriceRangeFilterQuery
        >();
        services.AddSingleton<
            IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>,
            AdvertisementTextSearchQuery
        >();
    }
}
