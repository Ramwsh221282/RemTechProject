using System.Data;
using System.Text;
using Dapper;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;
using RemTech.Infrastructure.PostgreSql.Configuration;
using RemTech.Infrastructure.PostgreSql.Shared;
using RemTech.Shared.SDK.CqrsPattern.Queries;
using RemTech.Shared.SDK.Utils;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisements;

public sealed class GetAdvertisementsQueryHandler(ConnectionStringFactory factory)
    : IQueryHandler<GetAdvertisementsQuery, PaginatedDaoResponse<AdvertisementDao>>
{
    private readonly ConnectionStringFactory _factory = factory;

    public async Task<PaginatedDaoResponse<AdvertisementDao>> Handle(
        GetAdvertisementsQuery query,
        CancellationToken ct = default
    )
    {
        SortOptions? sorting = query.Sort;
        FilterOptions? filter = query.Filter;
        PaginationOptions pagination = query.Pagination;

        StringBuilder dataQueryBuilder = new("SELECT * FROM advertisements WHERE 1=1 ");
        StringBuilder countQueryBuilder = new("SELECT COUNT(*) FROM advertisements WHERE 1=1 ");
        DynamicParameters dataParameters = new();
        DynamicParameters countParameters = new();

        if (filter != null)
        {
            if (filter.PriceFilter != null)
            {
                PriceFilter price = filter.PriceFilter;
                if (price.PriceFrom != null)
                {
                    dataQueryBuilder.Append(" AND price_value >= @price_from");
                    countQueryBuilder.Append(" AND price_value >= @price_from");
                    dataParameters.Add("price_from", price.PriceFrom);
                    countParameters.Add("price_from", price.PriceFrom);
                }
                if (price.PriceTo != null)
                {
                    dataQueryBuilder.Append(" AND price_value <= @price_to");
                    countQueryBuilder.Append(" AND price_value <= @price_to");
                    dataParameters.Add("price_to", price.PriceTo);
                    countParameters.Add("price_to", price.PriceTo);
                }
            }
            if (filter.AddressFilter != null)
            {
                string addressWord = filter
                    .AddressFilter.Address.CleanFromNewLines()
                    .CleanFromPunctuation()
                    .CleanFromExtraSpaces()
                    .Trim();
                dataParameters.Add("address", addressWord);
                countParameters.Add("address", addressWord);
                dataQueryBuilder.Append(
                    " AND to_tsvector(lower(address)) @@ plainto_tsquery(lower(@address))"
                );
                countQueryBuilder.Append(
                    " AND to_tsvector(lower(address)) @@ plainto_tsquery(lower(@address))"
                );
            }
            if (filter.TextFilter != null)
            {
                string formatted = filter
                    .TextFilter.Text.CleanFromPunctuation()
                    .CleanFromNewLines()
                    .CleanFromExtraSpaces()
                    .Trim();

                dataQueryBuilder.Append(
                    " AND to_tsvector(lower(title) || ' ' || lower(description)) @@ to_tsquery(lower(@word))"
                );
                countQueryBuilder.Append(
                    " AND to_tsvector(lower(title) || ' ' || lower(description)) @@ to_tsquery(lower(@word))"
                );
                dataParameters.Add("word", formatted);
                countParameters.Add("word", formatted);
            }
            if (filter.CharacteristicsFilter != null)
            {
                CharacteristicOption[] options = filter.CharacteristicsFilter.Characteristics;
                foreach (CharacteristicOption option in options)
                {
                    string name = option.Name;
                    string value = option.Value;

                    dataQueryBuilder.Append(
                        """ 
                        AND 
                        EXISTS ( 
                        SELECT 1 FROM jsonb_array_elements(characteristics) AS elem
                         WHERE elem->>'Name' = @name AND to_tsvector(lower(elem->>'Value')) @@ to_tsquery(lower(@value))
                          )
                        """
                    );

                    countQueryBuilder.Append(
                        """ 
                        AND 
                        EXISTS ( 
                        SELECT 1 FROM jsonb_array_elements(characteristics) AS elem
                         WHERE elem->>'Name' = @name AND to_tsvector(lower(elem->>'Value')) @@ to_tsquery(lower(@value))
                          )
                        """
                    );

                    dataParameters.Add("name", name);
                    dataParameters.Add("value", value);
                    countParameters.Add("name", name);
                    countParameters.Add("value", value);
                }
            }
        }
        if (sorting != null)
        {
            string mode = sorting.Mode;
            if (mode == "ASC")
            {
                dataQueryBuilder.Append(" ORDER BY price_value ASC");
            }
            if (mode == "DESC")
            {
                dataQueryBuilder.Append(" ORDER BY price_value DESC");
            }
        }

        int page = pagination.Page;
        int pageSize = pagination.PageSize;
        int offset = (page - 1) * pageSize;

        dataQueryBuilder.Append(" LIMIT @limit OFFSET @offset");
        dataParameters.Add("limit", pageSize);
        dataParameters.Add("offset", offset);

        string dataSql = dataQueryBuilder.ToString();
        string countSql = countQueryBuilder.ToString();

        CommandDefinition dataCommand = new(dataSql, dataParameters, cancellationToken: ct);
        CommandDefinition countCommand = new(countSql, countParameters, cancellationToken: ct);

        Task<int> countTask = ExecuteCountQuery(countCommand);
        Task<AdvertisementDao[]> dataTask = ExecuteDataQuery(dataCommand);

        await Task.WhenAll(countTask, dataTask);

        int count = await countTask;
        AdvertisementDao[] data = await dataTask;

        return PaginatedDaoResponse<AdvertisementDao>.Create(data, ref count, pagination);
    }

    private async Task<int> ExecuteCountQuery(CommandDefinition command)
    {
        using IDbConnection connection = _factory.Create();
        return await connection.ExecuteScalarAsync<int>(command);
    }

    private async Task<AdvertisementDao[]> ExecuteDataQuery(CommandDefinition command)
    {
        using IDbConnection connection = _factory.Create();
        IEnumerable<AdvertisementDao> data = await connection.QueryAsync<AdvertisementDao>(command);
        AdvertisementDao[] array = [.. data];
        return array;
    }
}
