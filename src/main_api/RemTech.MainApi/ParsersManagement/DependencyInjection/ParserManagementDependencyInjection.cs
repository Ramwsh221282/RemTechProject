using RemTech.MainApi.ParsersManagement.Configurations;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTechCommon.Utils.DependencyInjectionHelpers;

namespace RemTech.MainApi.ParsersManagement.DependencyInjection;

[DependencyInjection]
public static class ParserManagementDependencyInjection
{
    [DependencyInjectionMethod]
    public static void RegisterParserDependencies(this IServiceCollection services)
    {
        ParserDataServiceConfiguration dataServiceConfiguration =
            ParserDataServiceConfiguration.Create();
        services.AddSingleton(dataServiceConfiguration);
        services.AddSingleton<DataServiceMessagerFactory>();
    }
}
