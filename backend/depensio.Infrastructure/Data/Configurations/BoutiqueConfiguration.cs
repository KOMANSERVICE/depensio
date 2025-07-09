namespace depensio.Infrastructure.Data.Configurations;

public class BoutiqueConfiguration : IEntityTypeConfiguration<Boutique>
{
    public void Configure(EntityTypeBuilder<Boutique> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasConversion(
            boutiqueId => boutiqueId.Value,
            dbId => BoutiqueId.Of(dbId)
            );

        //builder.HasMany(c => c.Capitals)
        //    .WithOne()
        //    .HasForeignKey(c => c.BoutiqueId);

    }
}