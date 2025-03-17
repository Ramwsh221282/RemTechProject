using System.Reflection;
using RemTech.MainApi.Common.Attributes;

namespace RemTech.MainApi.Common.Extensions;

public static class MinimalApiExtensions
{
    public static void MapAllEndpoints(this WebApplication app) =>
        Assembly
            .GetExecutingAssembly()
            .GetTypesWithEndpointAttributes()
            .GetMethodsWithEndpointAttributes()
            .InvokeMappingMethods(app);

    private static IEnumerable<Type> GetTypesWithEndpointAttributes(this Assembly assembly) =>
        assembly.GetTypes().Where(t => t.GetCustomAttribute<EndpointMappingAttribute>() != null);

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
