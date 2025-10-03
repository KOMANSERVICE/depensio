namespace depensio.Infrastructure.Data.Configurations;

public class PlanFeatureConfiguration : IEntityTypeConfiguration<PlanFeature>
{
    public void Configure(EntityTypeBuilder<PlanFeature> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                planFeatureId => planFeatureId.Value,
                dbId => PlanFeatureId.Of(dbId)
            )
            .ValueGeneratedOnAdd();

        // 🎯 Conversion propre pour FeatureId
        builder.Property(e => e.FeatureId)
            .HasConversion(
                featureId => featureId.Value,
                dbId => FeatureId.Of(dbId)
            )
            .IsRequired();

        // 🎯 Conversion propre pour PlanId
        builder.Property(e => e.PlanId)
            .HasConversion(
                planId => planId.Value,
                dbId => PlanId.Of(dbId)
            )
            .IsRequired();

        // ✅ Config explicite de la relation
        builder.HasOne(e => e.Feature)
            .WithMany(b => b.PlanFeatures)
            .HasForeignKey(e => e.FeatureId)
            .OnDelete(DeleteBehavior.Restrict);

        // 🎯 Conversion propre pour ProduitId
        builder.Property(e => e.PlanId)
            .HasConversion(
                planId => planId.Value,
                dbId => PlanId.Of(dbId)
            )
            .IsRequired();

        // ✅ Config explicite de la relation
        builder.HasOne(e => e.Plan)
            .WithMany(b => b.PlanFeatures)
            .HasForeignKey(e => e.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}