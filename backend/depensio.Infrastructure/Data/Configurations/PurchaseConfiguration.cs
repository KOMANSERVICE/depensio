using depensio.Domain.Enums;

namespace depensio.Infrastructure.Data.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                purchaseId => purchaseId.Value,
                dbId => PurchaseId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

        builder.Property(e => e.BoutiqueId)
            .HasConversion(
                boutiqueId => boutiqueId.Value,
                dbId => BoutiqueId.Of(dbId)
            )
            .IsRequired();

        builder.Property(e => e.Status)
            .HasDefaultValue((int)PurchaseStatus.Approved)
            .IsRequired();

        builder.Property(e => e.TotalAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(e => e.ApprovedBy)
            .HasMaxLength(450);

        builder.Property(e => e.RejectionReason)
            .HasMaxLength(1000);

        builder.HasOne(e => e.Boutique)
            .WithMany(b => b.Purchases)
            .HasForeignKey(e => e.BoutiqueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.StatusHistory)
            .WithOne(h => h.Purchase)
            .HasForeignKey(h => h.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
