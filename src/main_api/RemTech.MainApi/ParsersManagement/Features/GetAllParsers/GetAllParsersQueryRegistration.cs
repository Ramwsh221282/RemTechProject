using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.DependencyInjectionHelpers;
using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MainApi.ParsersManagement.Features.GetAllParsers;

[DependencyInjection]
public static class GetAllParsersQueryRegistration
{
    [DependencyInjectionMethod]
    public static void Register(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<GetAllParsersQuery, Option<ParserResponse[]>>>(p =>
        {
            DataServiceMessagerFactory factory = p.GetRequiredService<DataServiceMessagerFactory>();
            DataServiceMessager messager = factory.Create();
            return new GetAllParsersQueryHandler(messager);
        });
    }
}
