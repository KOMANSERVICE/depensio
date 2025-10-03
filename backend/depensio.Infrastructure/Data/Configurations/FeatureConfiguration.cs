namespace depensio.Infrastructure.Data.Configurations;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasConversion(
            featureId => featureId.Value,
            dbId => FeatureId.Of(dbId)
        )
        .ValueGeneratedOnAdd();

    }
}