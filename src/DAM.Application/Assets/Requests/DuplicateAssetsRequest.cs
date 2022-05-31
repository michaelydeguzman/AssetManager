using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class DuplicateAssetsRequest : IRequest<HandlerResult<IEnumerable<AssetDto>>>
    {
        public List<int> AssetIds { get; private set; }

        public int FolderId { get; private set; }

        public string UserId { get; private set; }

        public DuplicateAssetsRequest(List<int> assetIds, int folderId, string userId)
        {
            AssetIds = assetIds;
            FolderId = folderId;
            UserId = userId;
        }
    }
}

