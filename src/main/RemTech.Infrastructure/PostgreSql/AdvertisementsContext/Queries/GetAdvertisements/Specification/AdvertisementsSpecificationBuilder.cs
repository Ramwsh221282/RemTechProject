using System.Text;
using Dapper;
using RemTech.Infrastructure.PostgreSql.Shared;
using RemTech.Shared.SDK.CqrsPattern.Queries;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisements.Specification;

public sealed class AdvertisementsSpecificationBuilder<TQuery>
    : ISpecificationBuilder<AdvertisementsSpecificationBuilder<TQuery>>
    where TQuery : IQuery
{
    private readonly StringBuilder _queryBuilder;
    private readonly DynamicParameters _parameters;

    public AdvertisementsSpecificationBuilder<TQuery> Instance => this;

    private AdvertisementsSpecificationBuilder(
        StringBuilder queryBuilder,
        DynamicParameters parameters
    )
    {
        _queryBuilder = queryBuilder;
        _parameters = parameters;
    }

    public static AdvertisementsSpecificationBuilder<TQuery> AdaptFactory(
        StringBuilder queryBuilder,
        DynamicParameters parameters
    ) => new(queryBuilder, parameters);

    public AdvertisementsSpecificationBuilder<TQuery> AddCharacteristicsTextSearch(
        CharacteristicOption option
    )
    {
        string name = option.Name;
        string value = option.Value;

        _queryBuilder.AppendLine(
            """
             AND EXISTS
             (SELECT 1 FROM jsonb_array_elements(characteristics) as elem
             WHERE elem->>'Name' = @name AND elem->>'Value' ILIKE CONCAT('%', @value, '%'))
            """
        );

        _parameters.Add("@name", name);
        _parameters.Add("@value", value);

        return this;
    }

    public CustomSqlQuery Create() => new CustomSqlQuery(_queryBuilder, _parameters);
}
