using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class UpdateAssetStatusDto
    {
        public List<int> AssetIds { get; set; }

        public string ModifiedById { get; set; }

        public bool IsSuccess { get; set; }

        public int Status { get; set; }

        public string Reason { get; set; }

        public int FolderId { get; set; }
    }
}
