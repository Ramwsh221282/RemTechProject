using RemTech.MongoDb.Service.Features.CharacteristicsManagement.Models;
using RemTechCommon.Utils.DependencyInjectionHelpers;

namespace RemTech.MongoDb.Service.Features.CharacteristicsManagement.Data;

[DependencyInjection]
public static class CharacteristicsDataDependencyInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddSingleton<CharacteristicsRepository>();
        services.AddSingleton<CharacteristicsCache>(p =>
        {
            var repository = p.GetRequiredService<CharacteristicsRepository>();
            Characteristic[] ctx = repository.Get().Result;
            return new CharacteristicsCache(ctx);
        });
    }
}
