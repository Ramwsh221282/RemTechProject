using System.Reflection;

namespace DromParser.Tests;

public sealed record A
{
    public B BValue { get; }

    private A(B bValue) => BValue = bValue;
}

public sealed record B
{
    public string Name { get; }

    public B(string name) => Name = name;
}

public class ReflectionTests
{
    [Fact]
    public void TestReflection()
    {
        Type type = typeof(A);
        ConstructorInfo[] constructors = type.GetConstructors(
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        B b = new B("Test");
        A a = (A)constructors[0].Invoke(new object[] { b });
        Assert.True(a.BValue == b);
    }
}
