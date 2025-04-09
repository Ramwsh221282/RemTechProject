using Microsoft.EntityFrameworkCore;
using RemTech.Domain.ParserContext;
using RemTech.Infrastructure.PostgreSql.Configuration;

namespace RemTech.Infrastructure.PostgreSql;

public sealed class ApplicationDbContext(ConnectionString connectionString) : DbContext
{
    private readonly ConnectionString _connectionString = connectionString;
    public DbSet<Parser> Parsers { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString.Value);
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
