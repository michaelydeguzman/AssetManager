using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Persistence.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasData(
                  new UserRole() { Id = 1, Name = "Admin", CanArchive = true, CanEdit = true, CanShare = true, CanApprove = true, CanAdd = true, CanDelete = true, CanMove = true, CanUpload = true , CanInvite = true, CanAccessAdmin = true, CanShareFolders=true, CanPinAsset =true},
                  new UserRole() { Id = 2, Name = "Company Admin", CanArchive = false, CanEdit = false, CanShare = false, CanApprove = false, CanAdd = false, CanDelete = false, CanMove = false, CanUpload = false, CanInvite = true, CanAccessAdmin = true, CanShareFolders = false, CanPinAsset = true },
                  new UserRole() { Id = 3, Name = "Subscriber", CanArchive = false, CanEdit = false, CanShare = false, CanApprove = false, CanAdd = false, CanDelete = false, CanMove = false, CanUpload = false, CanInvite = false, CanAccessAdmin = false, CanShareFolders = false, CanPinAsset = true },
                  new UserRole() { Id = 4, Name = "User", CanArchive = false, CanEdit = false, CanShare = true, CanApprove = false, CanAdd = false, CanDelete = false, CanMove = false, CanUpload = false, CanInvite = false, CanAccessAdmin = false, CanShareFolders = false, CanPinAsset = true }

              );
        }
    }
}
