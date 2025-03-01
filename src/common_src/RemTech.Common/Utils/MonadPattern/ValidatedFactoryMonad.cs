using RemTechCommon.Utils.ResultPattern;

namespace RemTechCommon.Utils.MonadPattern;

public sealed class ValidatedFactoryMonad<T>
{
    private List<string> _errors = [];
    private readonly T _property;

    private ValidatedFactoryMonad(T property)
    {
        _property = property;
    }

    public static ValidatedFactoryMonad<U> Start<U>(U property)
    {
        return new ValidatedFactoryMonad<U>(property);
    }

    public ValidatedFactoryMonad<U> Set<U>(Func<T, bool> predicate, U next, string message)
    {
        if (!predicate(_property))
            return new ValidatedFactoryMonad<U>(next);
        _errors.Add(message);
        return new ValidatedFactoryMonad<U>(next) { _errors = [.. _errors] };
    }

    public ValidatedFactoryMonad<T> Set(Func<T, bool> predicate, string message)
    {
        if (!predicate(_property))
            return new ValidatedFactoryMonad<T>(_property);
        _errors.Add(message);
        return new ValidatedFactoryMonad<T>(_property) { _errors = [.. _errors] };
    }

    public Result<U> Factory<U>(Func<U> factory)
    {
        if (_errors.Count == 0)
            return factory.Invoke();
        var message = string.Join("\n", _errors);
        return new Error(message);
    }
}
