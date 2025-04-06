using System.Reflection;

namespace RemTech.WebApi.Configuration.Endpoints;

public static class EndpointMapping
{
    public static void MapEndpoints(this WebApplication app)
    {
        typeof(EndpointMapping)
            .Assembly.GetTypesWithEndpointAttributes()
            .GetMethodsWithEndpointAttributes()
            .InvokeMappingMethods(app);
    }

    private static IEnumerable<Type> GetTypesWithEndpointAttributes(this Assembly assembly) =>
        assembly.GetTypes().Where(t => t.GetCustomAttribute<EndpointClassAttribute>() != null);

    private static IEnumerable<MethodInfo> GetMethodsWithEndpointAttributes(
        this IEnumerable<Type> types
    ) =>
        types.SelectMany(t =>
            t.GetMethods()
                .Where(m => m.GetCustomAttribute<EndpointMappingMethodAttribute>() != null)
        );

    private static void InvokeMappingMethods(
        this IEnumerable<MethodInfo> methods,
        WebApplication app
    )
    {
        foreach (var method in methods)
            method.Invoke(null, [app]);
    }
}
