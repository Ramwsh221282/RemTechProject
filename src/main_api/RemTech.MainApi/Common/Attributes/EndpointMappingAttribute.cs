namespace RemTech.MainApi.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EndpointMappingAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class EndpointMappingMethodAttribute : Attribute { }
