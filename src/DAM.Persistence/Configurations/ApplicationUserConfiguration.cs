using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAM.Persistence.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasMany(ac => ac.AssetCreated).WithOne(p => p.CreatedBy).HasForeignKey(a => a.CreatedById);
            builder.HasMany(am => am.AssetModified).WithOne(p => p.ModifiedBy).HasForeignKey(a => a.ModifiedById);
            builder.HasMany(fc => fc.FolderCreated).WithOne(p => p.CreatedBy).HasForeignKey(f => f.CreatedById);
            builder.HasMany(fm => fm.FolderModified).WithOne(p => p.ModifiedBy).HasForeignKey(f => f.ModifiedById);
            builder.HasMany(aa => aa.AssetAccountCreated).WithOne(p => p.CreatedBy).HasForeignKey(f => f.CreatedById);
            builder.HasMany(aa => aa.AssetAccountModified).WithOne(p => p.ModifiedBy).HasForeignKey(f => f.ModifiedById);
            builder.HasMany(fa => fa.FolderAccountCreated).WithOne(p => p.CreatedBy).HasForeignKey(f => f.CreatedById);
            builder.HasMany(fa => fa.FolderAccountModified).WithOne(p => p.ModifiedBy).HasForeignKey(f => f.ModifiedById);
            builder.HasMany(acr => acr.AssetCountryRegionCreated).WithOne(p => p.CreatedBy).HasForeignKey(f => f.CreatedById);
            builder.HasMany(acr => acr.AssetCountryRegionModified).WithOne(p => p.ModifiedBy).HasForeignKey(f => f.ModifiedById);
            builder.HasMany(acr => acr.FolderCountryRegionCreated).WithOne(p => p.CreatedBy).HasForeignKey(f => f.CreatedById);
            builder.HasMany(acr => acr.FolderCountryRegionModified).WithOne(p => p.ModifiedBy).HasForeignKey(f => f.ModifiedById);
            builder.HasMany(c => c.CommentsCreated).WithOne(p => p.CreatedBy).HasForeignKey(f => f.CreatedById);
            builder.HasMany(c => c.CommentsModified).WithOne(p => p.ModifiedBy).HasForeignKey(f => f.ModifiedById);
            builder.HasMany(com => com.CompanyCreated).WithOne(p => p.CreatedBy).HasForeignKey(a => a.CreatedById);
            builder.HasMany(com => com.CompanyModified).WithOne(p => p.ModifiedBy).HasForeignKey(a => a.ModifiedById);
        }
    }
}
