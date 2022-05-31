using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Requests
{
    public class ShareBulkAssetRequest : IRequest<HandlerResult<string>>
    {
        public string AssetsIds { get; private set; }
        public string FolderIds { get; private set; }

        public ShareBulkAssetRequest(string assetsIds, string folderIds)
        {
            AssetsIds = assetsIds;
            FolderIds = folderIds;
        }
    }
}
