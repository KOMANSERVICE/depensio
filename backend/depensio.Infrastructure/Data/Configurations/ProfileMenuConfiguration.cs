namespace depensio.Infrastructure.Data.Configurations;

public class ProfileMenuConfiguration : IEntityTypeConfiguration<ProfileMenu>
{
    public void Configure(EntityTypeBuilder<ProfileMenu> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                profileMenuId => profileMenuId.Value,
                dbId => ProfileMenuId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

        // 🎯 Conversion propre pour ProfileId
        builder.Property(e => e.ProfileId)
            .HasConversion(
                profileId => profileId.Value,
                dbId => ProfileId.Of(dbId)
            )
            .IsRequired();

        // 🎯 Conversion propre pour MenuId
        builder.Property(e => e.MenuId)
            .HasConversion(
                menuId => menuId.Value,
                dbId => MenuId.Of(dbId)
            )
            .IsRequired(false);

        // ✅ Config explicite de la relation
        builder.HasOne(e => e.Profiles)
            .WithMany(b => b.ProfileMenus)
            .HasForeignKey(e => e.ProfileId)
            .OnDelete(DeleteBehavior.Restrict);


        // ✅ Config explicite de la relation
        builder.HasOne(e => e.Menu)
            .WithMany(b => b.ProfileMenus)
            .HasForeignKey(e => e.MenuId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

    }
}