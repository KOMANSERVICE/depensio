namespace depensio.Infrastructure.Data.Configurations;

public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                purchaseItemId => purchaseItemId.Value,
                dbId => PurchaseItemId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

        // 🎯 Conversion propre pour PurchaseId
        builder.Property(e => e.PurchaseId)
            .HasConversion(
                purchaseId => purchaseId.Value,
                dbId => PurchaseId.Of(dbId)
            )
            .IsRequired();

        // 🎯 Conversion propre pour ProductId
        builder.Property(e => e.ProductId)
            .HasConversion(
                productId => productId.Value,
                dbId => ProductId.Of(dbId)
            )
            .IsRequired();

        // ✅ Config explicite de la relation
        builder.HasOne(e => e.Purchase)
            .WithMany(b => b.PurchaseItems)
            .HasForeignKey(e => e.PurchaseId)
            .OnDelete(DeleteBehavior.Restrict);

        // 🎯 Conversion propre pour ProduitId
        builder.Property(e => e.ProductId)
            .HasConversion(
                productId => productId.Value,
                dbId => ProductId.Of(dbId)
            )
            .IsRequired();

        // ✅ Config explicite de la relation
        builder.HasOne(e => e.Product)
            .WithMany(b => b.PurchaseItems)
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}