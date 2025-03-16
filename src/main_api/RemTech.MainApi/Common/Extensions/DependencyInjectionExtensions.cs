using System.Reflection;
using RemTech.MainApi.Common.Attributes;

namespace RemTech.MainApi.Common.Extensions;

public static class DependencyInjectionExtensions
{
    public static void RegisterServices(this IServiceCollection services) =>
        Assembly
            .GetExecutingAssembly()
            .GetTypesWithDependencyInjectionAttribute()
            .GetMethodsWithDependencyInjectionAttribute()
            .InvokeRegistrationMethods(services);

    private static IEnumerable<Type> GetTypesWithDependencyInjectionAttribute(
        this Assembly assembly
    ) =>
        assembly
            .GetTypes()
            .Where(t => t.GetCustomAttribute<DependencyInjectionAttribute>() != null);

    private static IEnumerable<MethodInfo> GetMethodsWithDependencyInjectionAttribute(
        this IEnumerable<Type> types
    ) =>
        types.SelectMany(t =>
            t.GetMethods().Where(m => m.GetCustomAttribute<ServicesRegistrationAttribute>() != null)
        );

    private static void InvokeRegistrationMethods(
        this IEnumerable<MethodInfo> methods,
        IServiceCollection services
    )
    {
        foreach (var method in methods)
            method.Invoke(null, [services]);
    }
}
