using Microsoft.Extensions.DependencyInjection;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.ResponseModels;
using RemTech.Shared.SDK.CqrsPattern.Queries;
using RemTech.Shared.SDK.DependencyInjection;
using RemTech.Shared.SDK.OptionPattern;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.Queries.GetByName;

[InjectionClass]
public static class GetParserByNameQueryDependencyInjection
{
    [InjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<
            IQueryHandler<GetParserByNameQuery, Option<ParserResponse>>,
            GetParserByNameQueryHandler
        >();
    }
}
