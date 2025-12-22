namespace depensio.Infrastructure.Data.Configurations;

public class PurchaseStatusHistoryConfiguration : IEntityTypeConfiguration<PurchaseStatusHistory>
{
    public void Configure(EntityTypeBuilder<PurchaseStatusHistory> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                dbId => PurchaseStatusHistoryId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

        builder.Property(e => e.PurchaseId)
            .HasConversion(
                purchaseId => purchaseId.Value,
                dbId => PurchaseId.Of(dbId)
            )
            .IsRequired();

        builder.Property(e => e.ToStatus)
            .IsRequired();

        builder.Property(e => e.Comment)
            .HasMaxLength(1000);

        builder.HasOne(e => e.Purchase)
            .WithMany(p => p.StatusHistory)
            .HasForeignKey(e => e.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
