namespace depensio.Infrastructure.Data.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasConversion(
                PlanId => PlanId.Value,
                dbId => PlanId.Of(dbId)
            )            
            .ValueGeneratedOnAdd();
    }
}