using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SharedParsersLibrary.Attributes;

namespace SharedParsersLibrary.DependencyInjection;

public static class InjectionClass
{
    public static void InjectParserDependencies(this IServiceCollection services)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        IEnumerable<Type> types = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<ParserDependencyInjectionAttribute>() != null);
        IEnumerable<MethodInfo> methods = types
            .SelectMany(t => t.GetMethods())
            .Where(m => m.GetCustomAttribute<ParserDependencyInjectionMethodAttribute>() != null);
        foreach (var method in methods)
            method.Invoke(null, [services]);
    }
}
