using Dapper.FluentMap;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Application.ParserContext.Contracts;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.EntityMappings;
using RemTech.Infrastructure.PostgreSql.Configuration;
using RemTech.Infrastructure.PostgreSql.ParserContext.EntityMappings;
using RemTech.Infrastructure.PostgreSql.ParserContext.Repositories;

namespace RemTech.Infrastructure.PostgreSql;

public static class PostgreSqlDependencyInjection
{
    public static void InjectPostgres(this IServiceCollection services)
    {
        ServiceDescriptor? descriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(ConnectionString)
        );

        if (descriptor == null)
            throw new ArgumentException(
                "Строка подключения к БД не проинициализирована в сервисе."
            );

        ConnectionString connectionString = (ConnectionString)descriptor.ImplementationInstance!;
        services.AddDbContext<ApplicationDbContext>(builder =>
            builder.UseNpgsql(connectionString.Value)
        );

        services.InjectRepositories();
    }

    public static void InjectConnectionString(this IServiceCollection services, string filePath)
    {
        ConnectionString connectionString = ConnectionStringReader.CreateFromFile(filePath);
        services.AddSingleton(connectionString);
        services.AddSingleton<ConnectionStringFactory>();
        InitializeDapperFluentMapping();
    }

    private static void InjectRepositories(this IServiceCollection services)
    {
        services.AddScoped<IParserWriteRepository, ParserWriteRepository>();
    }

    private static void InitializeDapperFluentMapping()
    {
        FluentMapper.Initialize(config =>
        {
            config.AddMap(new ParserDaoResponseMap());
            config.AddMap(new ParserProfileDaoResponseMap());
            config.AddMap(new TransportCharacteristicEntityMap());
        });
    }
}
