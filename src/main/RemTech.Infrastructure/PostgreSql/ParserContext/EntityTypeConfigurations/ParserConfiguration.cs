using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemTech.Domain.ParserContext;
using RemTech.Domain.ParserContext.ValueObjects;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.EntityTypeConfigurations;

public sealed class ParserConfiguration : IEntityTypeConfiguration<Parser>
{
    public void Configure(EntityTypeBuilder<Parser> builder)
    {
        builder.ToTable("parsers");

        builder.HasKey(p => p.Id);

        builder
            .Property(p => p.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Id, fromDb => ParserId.Dedicated(fromDb));

        builder
            .Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(ParserName.MAX_LENGTH)
            .HasConversion(toDb => toDb.Value, fromDb => ParserName.Create(fromDb));

        builder.HasIndex(p => p.Name).IsDescending().IsUnique();

        builder
            .HasMany(p => p.Profiles)
            .WithOne(pr => pr.Parser)
            .HasForeignKey(p => p.ParserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
