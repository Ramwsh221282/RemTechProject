using System.Reflection;
using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Abstractions.BsonClassMapping;
using RemTech.MongoDb.Service.Common.Abstractions.BsonSerialization;

namespace RemTech.MongoDb.Service.Configurations.MongoDbConfiguration;

public static class MongoDbConfigurationRegistration
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, string filePath)
    {
        var options = MongoDbConnectionOptions.ReadMongoDbConnectionOptionsFile(filePath);
        MongoClient client = options.CreateClient();
        services.AddSingleton(client);
        RegisterSerializers();
        RegisterClassMaps();

        return services;
    }

    private static void RegisterSerializers()
    {
        Assembly assembly = typeof(MongoDbConfigurationRegistration).Assembly;

        Type[] types = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t =>
                t.BaseType is { IsGenericType: true }
                && t.BaseType.GetGenericTypeDefinition() == typeof(CustomSerializer<>)
            )
            .ToArray();

        foreach (var type in types)
        {
            object instance = Activator.CreateInstance(type)!;
            MethodInfo method = instance.GetType().GetMethod("Register")!;
            method.Invoke(instance, null);
        }
    }

    private static void RegisterClassMaps()
    {
        Assembly assembly = typeof(MongoDbConfigurationRegistration).Assembly;

        Type[] types = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t =>
                t.BaseType is { IsGenericType: true }
                && t.BaseType.GetGenericTypeDefinition() == typeof(CustomClassMap<>)
            )
            .ToArray();

        foreach (var type in types)
        {
            object instance = Activator.CreateInstance(type)!;
            MethodInfo method = instance.GetType().GetMethod("Register")!;
            method.Invoke(instance, null);
        }
    }
}
