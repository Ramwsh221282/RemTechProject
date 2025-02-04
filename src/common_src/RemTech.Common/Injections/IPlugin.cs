using Microsoft.Extensions.DependencyInjection;

namespace RemTechCommon.Injections;

public interface IPlugin
{
    IServiceCollection Inject(IServiceCollection services);
}
