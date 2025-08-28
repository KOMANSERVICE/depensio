namespace depensio.Infrastructure.Data.Configurations;

public class ProductItemConfiguration : IEntityTypeConfiguration<ProductItem>
{
    public void Configure(EntityTypeBuilder<ProductItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                saleItemId => saleItemId.Value,
                dbId => ProductItemId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

        // 🎯 Conversion propre pour ProductId
        builder.Property(e => e.ProductId)
            .HasConversion(
                productId => productId.Value,
                dbId => ProductId.Of(dbId)
            )
            .IsRequired();

        // 🎯 Conversion propre pour ProduitId
        builder.Property(e => e.ProductId)
            .HasConversion(
                productId => productId.Value,
                dbId => ProductId.Of(dbId)
            )
            .IsRequired();

        // ✅ Config explicite de la relation
        builder.HasOne(e => e.Product)
            .WithMany(b => b.ProductItems)
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}