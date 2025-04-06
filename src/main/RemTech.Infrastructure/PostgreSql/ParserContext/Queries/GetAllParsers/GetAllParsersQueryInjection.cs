using Microsoft.Extensions.DependencyInjection;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.ResponseModels;
using RemTech.Shared.SDK.CqrsPattern.Queries;
using RemTech.Shared.SDK.DependencyInjection;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.Queries.GetAllParsers;

[InjectionClass]
public static class GetAllParsersQueryInjection
{
    [InjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<
            IQueryHandler<GetAllParsersQuery, ParserResponse[]>,
            GetAllParsersQueryHandler
        >();
    }
}
