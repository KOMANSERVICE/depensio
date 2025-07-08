namespace depensio.Infrastructure.Data.Configurations;

public class UsersBoutiqueConfiguration : IEntityTypeConfiguration<UsersBoutique>
{
    public void Configure(EntityTypeBuilder<UsersBoutique> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                usersBoutiqueId => usersBoutiqueId.Value,
                dbId => UsersBoutiqueId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

        // 🎯 Conversion propre pour BoutiqueId
        builder.Property(e => e.BoutiqueId)
            .HasConversion(
                boutiqueId => boutiqueId.Value,
                dbId => BoutiqueId.Of(dbId)
            )
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired();

        // ✅ Config explicite de la relation
        builder.HasOne(e => e.Boutique)
            .WithMany(b => b.UsersBoutiques)
            .HasForeignKey(e => e.BoutiqueId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}