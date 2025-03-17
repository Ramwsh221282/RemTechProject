using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Attributes;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Responses;

namespace RemTech.MainApi.ParsersManagement.Features.GetAllParsers;

[DependencyInjection]
public static class GetAllParsersQueryRegistration
{
    [ServicesRegistration]
    public static void Register(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetAllParsersQuery, ParserResponse[]>>(p =>
        {
            DataServiceMessagerFactory factory = p.GetRequiredService<DataServiceMessagerFactory>();
            DataServiceMessager messager = factory.Create();
            return new GetAllParsersQueryHandler(messager);
        });
    }
}
