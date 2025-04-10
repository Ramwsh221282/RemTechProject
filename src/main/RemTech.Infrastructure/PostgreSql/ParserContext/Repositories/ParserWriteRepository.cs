using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RemTech.Application.ParserContext.Contracts;
using RemTech.Domain.ParserContext;
using RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;
using RemTech.Domain.ParserContext.ValueObjects;
using RemTech.Shared.SDK.OptionPattern;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.Repositories;

public sealed class ParserWriteRepository(ApplicationDbContext context) : IParserWriteRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<Guid>> Add(Parser parser, CancellationToken ct = default)
    {
        await using IDbContextTransaction transcation =
            await _context.Database.BeginTransactionAsync(ct);

        await _context.Parsers.AddAsync(parser, ct);
        await _context.SaveChangesAsync(ct);
        await transcation.CommitAsync(ct);

        return parser.Id.Id;
    }

    public async Task Save(Parser parser, CancellationToken ct = default)
    {
        _context.Parsers.Attach(parser);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Option<Parser>> GetByName(string name, CancellationToken ct = default)
    {
        Result<ParserName> nameModel = ParserName.Create(name);
        if (nameModel.IsFailure)
            return Option<Parser>.None();

        Parser? parser = await _context
            .Parsers.Include(p => p.Profiles)
            .FirstOrDefaultAsync(p => p.Name == nameModel, ct);
        return parser == null ? Option<Parser>.None() : Option<Parser>.Some(parser);
    }
}
