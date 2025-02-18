namespace RemTechAvito.Core.Common.Specifications;

public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);
    ISpecification<T> And(ISpecification<T> other);
    ISpecification<T> Or(ISpecification<T> other);
    ISpecification<T> Not();
}

public abstract class SpecificationBase<T> : ISpecification<T>
{
    public abstract bool IsSatisfiedBy(T entity);

    public ISpecification<T> And(ISpecification<T> other) => new AndSpecification<T>(this, other);

    public ISpecification<T> Or(ISpecification<T> other) => new OrSpecification<T>(this, other);

    public ISpecification<T> Not() => new NotSpecification<T>(this);
}

public sealed class AndSpecification<T>(ISpecification<T> left, ISpecification<T> right)
    : SpecificationBase<T>
{
    public override bool IsSatisfiedBy(T entity) =>
        left.IsSatisfiedBy(entity) && right.IsSatisfiedBy(entity);
}

public sealed class OrSpecification<T>(ISpecification<T> left, ISpecification<T> right)
    : SpecificationBase<T>
{
    public override bool IsSatisfiedBy(T entity) =>
        left.IsSatisfiedBy(entity) || right.IsSatisfiedBy(entity);
}

public sealed class NotSpecification<T>(ISpecification<T> specification) : SpecificationBase<T>
{
    public override bool IsSatisfiedBy(T entity) => !specification.IsSatisfiedBy(entity);
}
