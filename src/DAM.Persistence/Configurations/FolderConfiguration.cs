using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAM.Persistence.Configurations
{
    public class FolderConfiguration : IEntityTypeConfiguration<Folder>
    {
        public void Configure(EntityTypeBuilder<Folder> builder)
        {
            builder.HasKey(e => e.Id );
            builder.ToTable("Folders");
            builder.HasData(
                    new Folder() { Id = 1, FolderName = "DAM", Description = "root"}
                );
        }
    }
}
