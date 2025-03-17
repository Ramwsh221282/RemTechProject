namespace RemTech.MainApi.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DependencyInjectionAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class ServicesRegistrationAttribute : Attribute { }
