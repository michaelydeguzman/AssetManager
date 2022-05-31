using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class FolderAccountMetaData : EntityBase
    {
        public int FolderId { get; set; }
        public Folder Folder { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
