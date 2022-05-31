using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class MoveAssetsDto
    {
        public List<int> AssetIds { get; set; }

        public int? FolderId { get; set; }

        public string ModifiedById { get; set; }

        public bool IsSuccess { get; set; }
    }
}
