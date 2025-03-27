using Rabbit.RPC.Server.Abstractions.Core;

namespace RemTech.MongoDb.Service.Configurations.RabbitMqListenerConfiguration;

public static class RabbitMqConfigurationRegistration
{
    public static IServiceCollection AddRabbitMqConfigurationOptions(
        this IServiceCollection services,
        string filePath
    )
    {
        var options = RabbitMqListenerOptions.ReadRabbitMqListenerOptionsFile(filePath);
        services.AddSingleton<IRabbitMqListenerOptions, RabbitMqListenerOptions>(_ => options);
        return services;
    }
}
