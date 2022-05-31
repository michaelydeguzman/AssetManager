using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Persistence.Configurations
{
    public class UserFolderConfiguration : IEntityTypeConfiguration<UserFolder>
    {
        public void Configure(EntityTypeBuilder<UserFolder> builder)
        {
            builder.HasKey(e => e.Id);
            //builder.HasData(
            //      new UserFolder() { Id = 1, UserId = 1, FolderId = 1},
            //      new UserFolder() { Id = 2, UserId = 2, FolderId = 1},
            //      new UserFolder() { Id = 3, UserId = 3, FolderId = 1},
            //      new UserFolder() { Id = 4, UserId = 4, FolderId = 1},
            //      new UserFolder() { Id = 5, UserId = 5, FolderId = 1}
            //  );
        }
    }
}
