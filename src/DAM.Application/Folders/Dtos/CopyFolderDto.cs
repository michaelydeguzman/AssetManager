using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Folders.Dtos
{
    public class CopyFolderDto
    {
        public int FolderId { get; set; }

        public int? ParentFolderId { get; set; }

        public string ModifiedById { get; set; }

        public bool IsSuccess { get; set; }

        public int OrderNumber { get; set; }
    }
}
