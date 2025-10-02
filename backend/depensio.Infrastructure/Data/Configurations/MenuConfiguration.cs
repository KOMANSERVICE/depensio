namespace depensio.Infrastructure.Data.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasConversion(
                menuId => menuId.Value,
                dbId => MenuId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

    }
}