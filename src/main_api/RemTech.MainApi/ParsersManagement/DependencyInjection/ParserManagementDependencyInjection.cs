using RemTech.MainApi.Common.Attributes;
using RemTech.MainApi.ParsersManagement.Configurations;
using RemTech.MainApi.ParsersManagement.Messages;

namespace RemTech.MainApi.ParsersManagement.DependencyInjection;

[DependencyInjection]
public static class ParserManagementDependencyInjection
{
    [ServicesRegistration]
    public static void RegisterParserDependencies(this IServiceCollection services)
    {
        ParserDataServiceConfiguration dataServiceConfiguration =
            ParserDataServiceConfiguration.Create();
        services.AddSingleton(dataServiceConfiguration);
        services.AddSingleton<DataServiceMessagerFactory>();
    }
}
