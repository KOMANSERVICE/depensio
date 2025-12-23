namespace depensio.Infrastructure.Data.Configurations;

public class SaleStatusHistoryConfiguration : IEntityTypeConfiguration<SaleStatusHistory>
{
    public void Configure(EntityTypeBuilder<SaleStatusHistory> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                dbId => SaleStatusHistoryId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

        builder.Property(e => e.SaleId)
            .HasConversion(
                saleId => saleId.Value,
                dbId => SaleId.Of(dbId)
            )
            .IsRequired();

        builder.Property(e => e.ToStatus)
            .IsRequired();

        builder.Property(e => e.Comment)
            .HasMaxLength(1000);

        builder.HasOne(e => e.Sale)
            .WithMany(s => s.StatusHistory)
            .HasForeignKey(e => e.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
