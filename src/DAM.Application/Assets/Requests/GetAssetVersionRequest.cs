using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class GetAssetVersionRequest : IRequest<HandlerResult<IEnumerable<AssetDto>>>
    {
        public int AssetVersionId { get; private set; }

        public int AssetId { get; private set; }

        public GetAssetVersionRequest(int assetId, int assetVersionId)
        {
            AssetId = assetId;
            AssetVersionId = assetVersionId;
        }
    }
}
