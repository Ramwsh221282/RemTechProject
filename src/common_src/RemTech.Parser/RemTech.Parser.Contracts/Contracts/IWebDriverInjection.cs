using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Parser.Contracts.Contracts;

public interface IWebDriverInjection
{
    public IServiceCollection Inject(IServiceCollection services);
}
