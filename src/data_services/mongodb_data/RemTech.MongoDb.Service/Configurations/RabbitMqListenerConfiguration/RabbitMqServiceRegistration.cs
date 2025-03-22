using Rabbit.RPC.Server.Abstractions.Core;

namespace RemTech.MongoDb.Service.Configurations.RabbitMqListenerConfiguration;

public static class RabbitMqServiceRegistration
{
    public static void RegisterRabbitMqService(this IServiceCollection services)
    {
        ServerRegistrationContext registration = new ServerRegistrationContext();
        registration.RegisterContractsFromEntryAssembly();
        registration.RegisterServer(services);
    }
}
