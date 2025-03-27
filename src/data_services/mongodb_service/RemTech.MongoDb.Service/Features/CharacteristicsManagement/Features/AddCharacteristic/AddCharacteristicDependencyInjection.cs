using RemTech.MongoDb.Service.Features.CharacteristicsManagement.Data;
using RemTech.MongoDb.Service.Features.CharacteristicsManagement.Features.AddCharacteristic.Decorators;
using RemTechCommon.Utils.CqrsPattern;
using RemTechCommon.Utils.DependencyInjectionHelpers;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MongoDb.Service.Features.CharacteristicsManagement.Features.AddCharacteristic;

[DependencyInjection]
public static class AddCharacteristicDependencyInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<AddCharacteristicRequest, Result>>(p =>
        {
            CharacteristicsRepository repository =
                p.GetRequiredService<CharacteristicsRepository>();
            CharacteristicsCache cache = p.GetRequiredService<CharacteristicsCache>();
            Serilog.ILogger logger = p.GetRequiredService<Serilog.ILogger>();
            AddCharacteristicRequestHandler h1 = new(repository, cache);
            AddCharacteristicRepositoryCheckDecorator h2 = new(h1, repository);
            AddCharacteristicCacheCheckDecorator h3 = new(h2, cache);
            AddCharacteristicLoggingDecorator h4 = new(h3, logger);
            return h4;
        });
    }
}
