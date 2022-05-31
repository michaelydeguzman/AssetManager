using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class GetImageAssetsRequest : IRequest<HandlerResult<IEnumerable<AssetVersionsDto>>>
    {

        public List<int> FolderIds { get; private set; }

        public GetImageAssetsRequest(List<int> folderIds)
        {
            
            FolderIds = folderIds;
        }
    }
}

