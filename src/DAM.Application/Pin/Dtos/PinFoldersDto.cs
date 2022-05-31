using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Pin.Dtos
{
    public class PinFoldersDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public int OrderNumber { get; set; }
        public int NewFolderId { get; set; }
        public int PreFolderId { get; set; }
        public List<int> OrderedFolderIds { get; set; }
    }
}
