using RemTech.Infrastructure.PostgreSql.Configuration;
using RemTech.Infrastructure.PostgreSql.ParserContext.DaoModels;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.GetByName;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.DatabaseSinking.Queries;

namespace SharedParsersLibrary.DatabaseSinking;

public sealed class ParserManagementFacade(ConnectionStringFactory factory)
{
    private readonly GetParserByNameQueryHandler _getParser = new(factory);
    private readonly MakeParserSleepingQuery _makeSleeping = new(factory);
    private readonly MakeParserWorkingQuery _makeWorking = new(factory);

    public async Task<Option<ParserDao>> GetParser(string name)
    {
        GetParserByNameQuery query = new(name);
        return await _getParser.Handle(query);
    }

    public async Task MakeSleeping(ParserProfileDao parser) =>
        await _makeSleeping.UpdateParserSleeping(parser);

    public async Task MakeWorking(ParserProfileDao parser) =>
        await _makeWorking.UpdateParserWorking(parser);
}
