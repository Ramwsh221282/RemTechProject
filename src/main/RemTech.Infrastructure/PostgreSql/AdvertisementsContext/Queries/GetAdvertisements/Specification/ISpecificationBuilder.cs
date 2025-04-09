namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisements.Specification;

public interface ISpecificationBuilder
{
    CustomSqlQuery Create();
}

public interface ISpecificationBuilder<TSpecificationBuilder> : ISpecificationBuilder
    where TSpecificationBuilder : ISpecificationBuilder
{
    TSpecificationBuilder Instance { get; }
}
