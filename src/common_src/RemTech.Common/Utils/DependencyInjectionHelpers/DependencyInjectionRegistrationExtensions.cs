using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RemTechCommon.Utils.DependencyInjectionHelpers;

public static class DependencyInjectionRegistrationExtensions
{
    public static void RegisterServices(this IServiceCollection services) =>
        AppDomain
            .CurrentDomain.GetAssemblies()
            .GetTypesWithDependencyInjectionAttribute()
            .GetMethodsWithDependencyInjectionAttribute()
            .CallRegistrationMethods(services);

    private static Type[] GetTypesWithDependencyInjectionAttribute(this Assembly[] assemblies) =>
        [
            .. assemblies
                .SelectMany(c => c.GetTypes())
                .Where(t => t.GetCustomAttribute<DependencyInjectionAttribute>() != null),
        ];

    private static MethodInfo[] GetMethodsWithDependencyInjectionAttribute(this Type[] types) =>
        [
            .. types
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttribute<DependencyInjectionMethodAttribute>() != null),
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
