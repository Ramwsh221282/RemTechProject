using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemTech.Domain.ParserContext.Entities.ParserProfiles;
using RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.EntityTypeConfigurations;

public sealed class ParserProfileConfiguration : IEntityTypeConfiguration<ParserProfile>
{
    public void Configure(EntityTypeBuilder<ParserProfile> builder)
    {
        builder.ToTable("parser_profiles");
        builder.HasKey(pr => pr.Id);

        builder
            .Property(pr => pr.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => ParserProfileId.Dedicated(fromDb));

        builder.Property(pr => pr.ParserId).HasColumnName("parser_id");

        builder
            .Property(pr => pr.Name)
            .HasColumnName("name")
            .HasMaxLength(ParserProfileName.MAX_NAME_LENGTH)
            .HasConversion(toDb => toDb.Value, fromDb => ParserProfileName.Create(fromDb));

        builder
            .Property(pr => pr.State)
            .HasColumnName("state")
            .HasMaxLength(20)
            .HasConversion(toDb => toDb.State, fromDb => ParserProfileState.Create(fromDb));

        builder.ComplexProperty(
            pr => pr.Schedule,
            cpb =>
            {
                cpb.Property(sc => sc.RepeatEveryUnixSeconds).HasColumnName("repeat_every_seconds");
                cpb.Property(sc => sc.NextRunUnixSeconds).HasColumnName("next_run_unix_seconds");
            }
        );

        builder.OwnsOne(
            pr => pr.Links,
            onb =>
            {
                onb.ToJson();
                onb.OwnsMany(
                    li => li.Links,
                    lionb =>
                    {
                        lionb
                            .Property(link => link.Link)
                            .HasConversion(
                                toDb => toDb,
                                fromDb => ParserProfileLink.Create(fromDb).Value.Link
                            );
                    }
                );
            }
        );

        builder.HasIndex(pr => pr.Name).IsDescending();
    }
}
