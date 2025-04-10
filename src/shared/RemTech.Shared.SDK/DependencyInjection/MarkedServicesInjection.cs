using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Shared.SDK.DependencyInjection;

public static class MarkedServicesInjection
{
    public static void InjectMarkedServices(this IServiceCollection services) =>
        AppDomain
            .CurrentDomain.GetAssemblies()
            .GetTypesWithDependencyInjectionAttribute()
            .GetMethodsWithDependencyInjectionAttribute()
            .CallRegistrationMethods(services);

    private static Type[] GetTypesWithDependencyInjectionAttribute(this Assembly[] assemblies) =>
        [
            .. assemblies
                .SelectMany(c => c.GetTypes())
                .Where(t => t.GetCustomAttribute<InjectionClassAttribute>() != null),
        ];

    private static MethodInfo[] GetMethodsWithDependencyInjectionAttribute(this Type[] types) =>
        [
            .. types
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttribute<InjectionMethodAttribute>() != null),
        ];

    private static void CallRegistrationMethods(
        this MethodInfo[] methods,
        IServiceCollection services
    )
    {
        foreach (MethodInfo method in methods)
        {
            method.Invoke(null, [services]);
        }
    }
}
