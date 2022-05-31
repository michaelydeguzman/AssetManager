using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }

        public virtual ICollection<AssetAccountMetaData> AssetAccounts { get; set; }

        public virtual ICollection<FolderAccountMetaData> FolderAccounts { get; set; }
    }
}
