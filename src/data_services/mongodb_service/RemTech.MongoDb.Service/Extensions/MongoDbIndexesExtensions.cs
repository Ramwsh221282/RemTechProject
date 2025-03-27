using System.Reflection;
using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Attributes;

namespace RemTech.MongoDb.Service.Extensions;

public static class MongoDbIndexesExtensions
{
    public static void ApplyIndexes(this MongoClient client)
    {
        var methods = AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<IndexModelAttribute>() != null)
            .SelectMany(t => t.GetMethods())
            .Where(m => m.GetCustomAttribute<IndexModelMethodAttribute>() != null);

        foreach (var method in methods)
        {
            method.Invoke(null, [client]);
        }
    }
}
