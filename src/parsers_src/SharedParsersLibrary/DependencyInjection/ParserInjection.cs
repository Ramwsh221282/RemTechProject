using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SharedParsersLibrary.Attributes;

namespace SharedParsersLibrary.DependencyInjection;

public static class ParserInjection
{
    public static void InjectParserDependencies(this IServiceCollection services)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        IEnumerable<Type> types = assemblies.GetTypesWithMarkedAttribute();
        IEnumerable<MethodInfo> methods = types.GetInjectionMethods();
        services.InvokeInjectionMethods(methods);
    }

    private static IEnumerable<Type> GetTypesWithMarkedAttribute(this Assembly[] assemblies) =>
        assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<ParserDependencyInjectionAttribute>() != null);

    private static IEnumerable<MethodInfo> GetInjectionMethods(this IEnumerable<Type> types) =>
        types
            .SelectMany(t => t.GetMethods())
            .Where(m => m.GetCustomAttribute<ParserDependencyInjectionMethodAttribute>() != null);

    private static void InvokeInjectionMethods(
        this IServiceCollection services,
        IEnumerable<MethodInfo> methods
    )
    {
        foreach (MethodInfo method in methods)
            method.Invoke(null, [services]);
    }
}
