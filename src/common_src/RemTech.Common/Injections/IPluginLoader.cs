using Microsoft.Extensions.DependencyInjection;

namespace RemTechCommon.Injections;

public interface IPluginLoader
{
    IServiceCollection RegisterServices(IServiceCollection services);
}
