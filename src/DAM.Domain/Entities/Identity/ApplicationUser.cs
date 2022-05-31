using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {

        // Extension Properties
        public string ImageFileExtension { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
        public bool Deleted { get; set; }
        public bool Active { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
        // Relationships
        public virtual ICollection<Asset> AssetCreated { get; set; }
        public virtual ICollection<Asset> AssetModified { get; set; }

        public virtual ICollection<Folder> FolderCreated { get; set; }
        public virtual ICollection<Folder> FolderModified { get; set; }

        public virtual ICollection<Company> CompanyCreated { get; set; }
        public virtual ICollection<Company> CompanyModified { get; set; }

        public virtual ICollection<AssetAccountMetaData> AssetAccountCreated { get; set; }
        public virtual ICollection<AssetAccountMetaData> AssetAccountModified { get; set; }

        public virtual ICollection<FolderAccountMetaData> FolderAccountCreated { get; set; }
        public virtual ICollection<FolderAccountMetaData> FolderAccountModified { get; set; }

        public virtual ICollection<AssetCountryRegionMetaData> AssetCountryRegionCreated { get; set; }
        public virtual ICollection<AssetCountryRegionMetaData> AssetCountryRegionModified { get; set; }

        public virtual ICollection<FolderCountryRegionMetaData> FolderCountryRegionCreated { get; set; }
        public virtual ICollection<FolderCountryRegionMetaData> FolderCountryRegionModified { get; set; }

        public virtual ICollection<Comment> CommentsCreated { get; set; }
        public virtual ICollection<Comment> CommentsModified { get; set; }

        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
    }
}
