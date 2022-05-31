using System;
using System.Collections.Generic;

namespace DAM.Domain.Entities
{
    public class Comment : EntityBase
    {
        public int? AssetId { get; set; }
        public Asset Asset { get; set; }

        public int? FolderId { get; set; }
        public Folder Folder { get; set; }

        public string CommentText { get; set; }

        public bool Deleted { get; set; }
    }
}
