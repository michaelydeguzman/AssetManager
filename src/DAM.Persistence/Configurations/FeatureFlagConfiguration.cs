using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAM.Persistence.Configurations
{
    public class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
    {
        public void Configure(EntityTypeBuilder<FeatureFlag> builder)
        {
            builder.HasKey(e => e.Id);
            builder.ToTable("FeatureFlags");
            builder.HasData(
                  new FeatureFlag() { Id = 1, FeatureFlagNumber = 1, FeatureFlagName = "Approvals", IsActivated = true},
                  new FeatureFlag() { Id = 2, FeatureFlagNumber = 2, FeatureFlagName = "Partnership", IsActivated = true },
                  new FeatureFlag() { Id = 3, FeatureFlagNumber = 3, FeatureFlagName = "AssetVersioning", IsActivated = true },
                  new FeatureFlag() { Id = 4, FeatureFlagNumber = 4, FeatureFlagName = "VideoIndexer", IsActivated = true },
                  new FeatureFlag() { Id = 5, FeatureFlagNumber = 5, FeatureFlagName = "Tagging", IsActivated = true },
                  new FeatureFlag() { Id = 6, FeatureFlagNumber = 6, FeatureFlagName = "PowerAnnotate", IsActivated = true },
                  new FeatureFlag() { Id = 7, FeatureFlagNumber = 7, FeatureFlagName = "Archive", IsActivated = true },
                  new FeatureFlag() { Id = 8, FeatureFlagNumber = 8, FeatureFlagName = "AuditTrail", IsActivated = true },
                  new FeatureFlag() { Id = 9, FeatureFlagNumber = 9, FeatureFlagName = "Report", IsActivated = true },
                  new FeatureFlag() { Id = 10, FeatureFlagNumber = 10, FeatureFlagName = "PromoteMRM", IsActivated = false },
                  new FeatureFlag() { Id = 11, FeatureFlagNumber = 11, FeatureFlagName = "CheckDuplicate", IsActivated = true }
            );
        }
    }
}
