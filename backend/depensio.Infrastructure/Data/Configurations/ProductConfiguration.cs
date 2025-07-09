namespace depensio.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                productId => productId.Value,
                dbId => ProductId.Of(dbId)
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
            .WithMany(b => b.Products)
            .HasForeignKey(e => e.BoutiqueId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}