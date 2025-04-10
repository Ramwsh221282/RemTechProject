namespace RemTech.Infrastructure.PostgreSql.Shared;

public interface ISpecificationBuilder
{
    CustomSqlQuery Create();
}

public interface ISpecificationBuilder<TSpecificationBuilder> : ISpecificationBuilder
    where TSpecificationBuilder : ISpecificationBuilder
{
    TSpecificationBuilder Instance { get; }
}
