namespace depensio.Infrastructure.Data.Configurations;

public class BoutiqueSettingConfiguration : IEntityTypeConfiguration<BoutiqueSetting>
{
    public void Configure(EntityTypeBuilder<BoutiqueSetting> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                boutiqueSettingId => boutiqueSettingId.Value,
                dbId => BoutiqueSettingId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

        // 🎯 Conversion propre pour BoutiqueId
        builder.Property(e => e.BoutiqueId)
            .HasConversion(
                boutiqueId => boutiqueId.Value,
                dbId => BoutiqueId.Of(dbId)
            )
            .IsRequired();

        // ✅ Config explicite de la relation
        builder.HasOne(e => e.Boutique)
            .WithMany(b => b.BoutiqueSettings)
            .HasForeignKey(e => e.BoutiqueId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}