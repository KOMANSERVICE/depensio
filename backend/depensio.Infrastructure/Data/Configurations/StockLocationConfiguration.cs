namespace depensio.Infrastructure.Data.Configurations;

public class StockLocationConfiguration : IEntityTypeConfiguration<StockLocation>
{
    public void Configure(EntityTypeBuilder<StockLocation> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                stockLocationId => stockLocationId.Value,
                dbId => StockLocationId.Of(dbId)
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
            .WithMany(b => b.StockLocations)
            .HasForeignKey(e => e.BoutiqueId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}