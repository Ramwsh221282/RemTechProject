using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.DependencyInjectionHelpers;
using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MainApi.ParsersManagement.Features.GetParser;

[DependencyInjection]
public static class GetParserQueryRegistration
{
    [DependencyInjectionMethod]
    public static void Register(this IServiceCollection services)
    {
        services.AddScoped<
            IRequestHandler<GetParserQuery, Option<ParserResponse>>,
            GetParserQueryHandler
        >();
    }
}
