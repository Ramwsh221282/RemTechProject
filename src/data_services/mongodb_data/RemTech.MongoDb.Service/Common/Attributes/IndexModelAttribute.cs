namespace RemTech.MongoDb.Service.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class IndexModelAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public sealed class IndexModelMethodAttribute : Attribute { }
