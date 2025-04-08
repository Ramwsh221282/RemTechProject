using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemTech.Domain.AdvertisementsContext;
using RemTech.Domain.AdvertisementsContext.ValueObjects;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.EntityTypeConfigurations;

public sealed class AdvertisementEntityTypeConfiguration : IEntityTypeConfiguration<Advertisement>
{
    public void Configure(EntityTypeBuilder<Advertisement> builder)
    {
        builder.ToTable("advertisements");
        builder.HasKey(ad => ad.Id);

        builder
            .Property(ad => ad.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => AdvertisementId.Dedicated(fromDb).Value);

        builder.ComplexProperty(
            ad => ad.Text,
            cpb =>
            {
                cpb.Property(t => t.Title).HasColumnName("title");
                cpb.Property(t => t.Description).HasColumnName("description");
            }
        );

        builder.ComplexProperty(
            ad => ad.Price,
            cpb =>
            {
                cpb.Property(p => p.Value).HasColumnName("price_value");
                cpb.Property(p => p.Extra)
                    .HasColumnName("price_extra")
                    .HasMaxLength(AdvertisementPriceInformation.MAX_PRICE_EXTRA_LENGTH);
            }
        );

        builder.ComplexProperty(
            ad => ad.Scraper,
            cpb =>
            {
                cpb.Property(s => s.ScraperName)
                    .HasColumnName("scraper_name")
                    .HasMaxLength(AdvertisementScraperInformation.MAX_SCRAPER_NAME_LENGTH);
                cpb.Property(s => s.PublishedBy)
                    .HasColumnName("published_by")
                    .HasMaxLength(AdvertisementScraperInformation.MAX_SCRAPER_NAME_LENGTH);
                cpb.Property(s => s.SourceUrl).HasColumnName("source_url");
            }
        );

        builder.OwnsOne(
            ad => ad.Photos,
            onb =>
            {
                onb.ToJson("photos");
                onb.OwnsMany(
                    p => p.Photos,
                    onbP =>
                    {
                        onbP.Property(ph => ph.Source).HasColumnName("photo");
                    }
                );
            }
        );

        builder.OwnsOne(
            ad => ad.Characteristics,
            onb =>
            {
                onb.ToJson("characteristics");
                onb.OwnsMany(
                    c => c.Characteristics,
                    onbC =>
                    {
                        onbC.Property(c => c.Name).HasColumnName("characteristic_name");
                        onbC.Property(c => c.Value).HasColumnName("characteristic_value");
                    }
                );
            }
        );
    }
}
