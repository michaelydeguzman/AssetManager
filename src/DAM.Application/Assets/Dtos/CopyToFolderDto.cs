using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class CopyToFolderDto
    {
        public List<int> AssetIds { get; set; }

        public int FolderId { get; set; }
    }
}
