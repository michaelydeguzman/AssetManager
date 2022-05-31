using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Folders.Dtos
{
    public class DeleteFoldersDto
    {
        public List<int> FolderIds { get; set; }

        public string DeletedBy { get; set; }
        public bool IsSuccess { get; set; }
    }
}
