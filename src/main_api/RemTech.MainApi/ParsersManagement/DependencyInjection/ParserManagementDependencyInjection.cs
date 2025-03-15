using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.ParsersManagement.Configurations;
using RemTech.MainApi.ParsersManagement.Messages.DataServiceMessages;
using RemTech.MainApi.ParsersManagement.Services;

namespace RemTech.MainApi.ParsersManagement.DependencyInjection;

public static class ParserManagementDependencyInjection
{
    public static void RegisterParserDependencies(this IServiceCollection services)
    {
        services.AddScoped<ParserDataServiceMessager>(_ =>
        {
            ParserDataServiceConfiguration conf = ParserDataServiceConfiguration.Create();
            MultiCommunicationPublisher publisher = new MultiCommunicationPublisher(
                conf.QueueName,
                conf.HostName,
                conf.UserName,
                conf.Password
            );
            return new ParserDataServiceMessager(publisher);
        });
        services.AddScoped<ParserManagementService>();
    }
}
