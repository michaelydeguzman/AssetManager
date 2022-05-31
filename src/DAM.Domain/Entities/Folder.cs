using System;
using System.Collections.Generic;

namespace DAM.Domain.Entities
{
    public class Folder : EntityBase
    {
        public string FolderName { get; set; }

        public string Description { get; set; }

        public int? ParentFolderId { get; set; }
        public Folder ParentFolder { get; set; }

        public bool Deleted { get; set; }

        public virtual ICollection<Asset> Assets { get; set; }

        //public virtual ICollection<Folder> Subfolders { get; set; }

        public virtual ICollection<FolderAccountMetaData> FolderAccounts { get; set; }

        public virtual ICollection<FolderCountryRegionMetaData> FolderCountryRegions { get; set; }

        public virtual ICollection<Comment> FolderComments { get; set; }

        public virtual ICollection<Company> Company { get; set; }

        public int OrderNumber { get; set; }
    }
}
