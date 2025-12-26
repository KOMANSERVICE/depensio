using depensio.Domain.Enums;

namespace depensio.Infrastructure.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                saleId => saleId.Value,
                dbId => SaleId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

        builder.Property(e => e.BoutiqueId)
            .HasConversion(
                boutiqueId => boutiqueId.Value,
                dbId => BoutiqueId.Of(dbId)
            )
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<int>()
            .HasDefaultValue(SaleStatus.Validated)
            .IsRequired();

        builder.Property(e => e.CancellationReason)
            .HasMaxLength(500);

        // Treasury integration fields (no SQL FK - references external Treasury microservice)
        builder.Property(e => e.PaymentMethodId);
        builder.Property(e => e.AccountId);
        builder.Property(e => e.CategoryId);
        builder.Property(e => e.CashFlowId);
        builder.Property(e => e.ReversalCashFlowId);
        builder.Property(e => e.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasOne(e => e.Boutique)
            .WithMany(b => b.Sales)
            .HasForeignKey(e => e.BoutiqueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}