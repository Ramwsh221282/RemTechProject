using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Attributes;
using RemTech.MainApi.ParsersManagement.Responses;

namespace RemTech.MainApi.ParsersManagement.Features.GetParser;

[DependencyInjection]
public static class GetParserQueryRegistration
{
    [ServicesRegistration]
    public static void Register(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetParserQuery, ParserResponse>, GetParserQueryHandler>();
    }
}
