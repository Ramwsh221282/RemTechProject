namespace RemTechCommon.Utils.OptionPattern;

public abstract record Option<T>
{
    public bool HasValue => (this is Some<T>);

    public T Value
    {
        get
        {
            if (this is Some<T> some)
                return some.SomeValue;
            throw new ArgumentException("Option has no value");
        }
    }

    public static Option<T> Some(T value) => Some<T>.Create(value);

    public static Option<T> None() => new None<T>();
}

public sealed record None<T> : Option<T>
{
    public static Option<T> Create() => new None<T>();

    public static Option<U> Create<U>() => new None<U>();
}

public sealed record Some<T> : Option<T>
{
    public T SomeValue { get; }

    private Some(T someValue) => SomeValue = someValue;

    public static Option<T> Create(T value) => new Some<T>(value);

    public static Option<U> Create<U>(U value) => new Some<U>(value);
}
