namespace depensio.Infrastructure.Data.Configurations;

public class PlanMenuConfiguration : IEntityTypeConfiguration<PlanMenu>
{
    public void Configure(EntityTypeBuilder<PlanMenu> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                planMenuId => planMenuId.Value,
                dbId => PlanMenuId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

        // 🎯 Conversion propre pour PlanId
        builder.Property(e => e.PlanId)
            .HasConversion(
                planId => planId.Value,
                dbId => PlanId.Of(dbId)
            )
            .IsRequired();

        // 🎯 Conversion propre pour MenuId
        builder.Property(e => e.MenuId)
            .HasConversion(
                menuId => menuId.Value,
                dbId => MenuId.Of(dbId)
            )
            .IsRequired();

        // ✅ Config explicite de la relation
        builder.HasOne(e => e.Plan)
            .WithMany(b => b.PlanMenus)
            .HasForeignKey(e => e.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        // 🎯 Conversion propre pour ProduitId
        builder.Property(e => e.MenuId)
            .HasConversion(
                menuId => menuId.Value,
                dbId => MenuId.Of(dbId)
            )
            .IsRequired();

        // ✅ Config explicite de la relation
        builder.HasOne(e => e.Menu)
            .WithMany(b => b.PlanMenus)
            .HasForeignKey(e => e.MenuId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}