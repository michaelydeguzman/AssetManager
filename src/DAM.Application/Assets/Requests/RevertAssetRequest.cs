using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class RevertAssetRequest : IRequest<HandlerResult<IEnumerable<AssetDto>>>
    {
        public int AssetId { get; set; }
        public int VersionAssetId { get; private set; }

        public RevertAssetRequest(int assetId, int versionId)
        {
            AssetId = assetId;
            VersionAssetId = versionId;
        }
    }
}
