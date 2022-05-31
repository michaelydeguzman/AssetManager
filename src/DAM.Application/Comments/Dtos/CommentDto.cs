using DAM.Application.Folders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class CommentDto
    {
        public int? Id { get; set; }

        public int? AssetId { get; set; }

        public AssetDto Asset { get; set; }

        public int? FolderId { get; set; }

        public FolderDto Folder { get; set; }

        public string CommentText { get; set; }

        public bool Deleted { get; set; }

        public string CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
