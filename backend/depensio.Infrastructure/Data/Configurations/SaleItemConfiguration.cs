namespace depensio.Infrastructure.Data.Configurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                saleItemId => saleItemId.Value,
                dbId => SaleItemId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

        // 🎯 Conversion propre pour SaleId
        builder.Property(e => e.SaleId)
            .HasConversion(
                saleId => saleId.Value,
                dbId => SaleId.Of(dbId)
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
        builder.HasOne(e => e.Sale)
            .WithMany(b => b.SaleItems)
            .HasForeignKey(e => e.SaleId)
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
            .WithMany(b => b.SaleItems)
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}