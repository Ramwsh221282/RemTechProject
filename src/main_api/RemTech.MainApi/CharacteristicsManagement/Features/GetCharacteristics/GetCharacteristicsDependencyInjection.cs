using RemTech.MainApi.CharacteristicsManagement.Responses;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTechCommon.Utils.DependencyInjectionHelpers;
using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MainApi.CharacteristicsManagement.Features.GetCharacteristics;

[DependencyInjection]
public static class GetCharacteristicsDependencyInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<
            IRequestHandler<GetCharacteristicsRequest, Option<CharacteristicResponse[]>>
        >(p =>
        {
            DataServiceMessagerFactory factory = p.GetRequiredService<DataServiceMessagerFactory>();
            DataServiceMessager messager = factory.Create();
            return new GetCharacteristicsRequestHandler(messager);
        });
    }
}
