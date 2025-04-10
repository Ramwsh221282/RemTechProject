using System.Data;
using Dapper;
using RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;
using RemTech.Infrastructure.PostgreSql.Configuration;
using RemTech.Infrastructure.PostgreSql.ParserContext.DaoModels;

namespace SharedParsersLibrary.DatabaseSinking.Queries;

public sealed class MakeParserWorkingQuery(ConnectionStringFactory factory)
{
    private const string Sql = """
        UPDATE parser_profiles
        SET state = @state, next_run_unix_seconds = @next_run_unix_seconds
        WHERE id = @id
        """;
    private readonly ConnectionStringFactory _factory = factory;

    public async Task UpdateParserWorking(ParserProfileDao profileDao)
    {
        Guid id = profileDao.Id;
        string state = ParserProfileState.Working.State;
        long nextRunUnixSeconds = DateTimeOffset.UtcNow.Second + profileDao.NextRunUnixSeconds;

        using IDbConnection connection = _factory.Create();
        await connection.ExecuteAsync(
            Sql,
            new
            {
                id = id,
                state = state,
                next_run_unix_seconds = nextRunUnixSeconds,
            }
        );
    }
}
