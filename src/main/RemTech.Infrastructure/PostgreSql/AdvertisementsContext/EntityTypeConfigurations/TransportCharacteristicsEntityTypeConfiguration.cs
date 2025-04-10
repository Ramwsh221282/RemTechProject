using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemTech.Application.AdvertisementsContext.Models.CharacteristicsManagement;
using RemTech.Domain.AdvertisementsContext.ValueObjects;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.EntityTypeConfigurations;

public sealed class TransportCharacteristicsEntityTypeConfiguration
    : IEntityTypeConfiguration<TransportCharacteristic>
{
    public void Configure(EntityTypeBuilder<TransportCharacteristic> builder)
    {
        builder.ToTable("characteristics");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("id");

        builder
            .Property(c => c.Name)
            .HasMaxLength(AdvertisementCharacteristic.MAX_CHARACTERISTIC_LENGTH)
            .HasColumnName("name");
    }
}
