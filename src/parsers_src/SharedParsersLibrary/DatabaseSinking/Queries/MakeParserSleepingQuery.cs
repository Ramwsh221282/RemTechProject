using System.Data;
using Dapper;
using RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;
using RemTech.Infrastructure.PostgreSql.Configuration;
using RemTech.Infrastructure.PostgreSql.ParserContext.DaoModels;

namespace SharedParsersLibrary.DatabaseSinking.Queries;

public sealed class MakeParserSleepingQuery(ConnectionStringFactory factory)
{
    private const string Sql = """
        UPDATE parser_profiles
        SET state = @state
        WHERE id = @id
        """;
    private readonly ConnectionStringFactory _factory = factory;

    public async Task UpdateParserSleeping(ParserProfileDao profileDao)
    {
        Guid id = profileDao.Id;
        string state = ParserProfileState.Sleeping.State;
        using IDbConnection connection = _factory.Create();
        await connection.ExecuteAsync(Sql, new { id = id, state = state });
    }
}
