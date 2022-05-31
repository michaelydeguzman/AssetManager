using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;

namespace DAM.Application.Assets.Requests
{
    public class RemoveAssetThumbnailRequest : IRequest<HandlerResult<AssetThumbnailDto>>
    {

        public AssetThumbnailDto AssetThumbnailDto { get; private set; }

        public string UserId { get; private set; }

        public RemoveAssetThumbnailRequest(AssetThumbnailDto asset, string userId)
        {
            AssetThumbnailDto = asset;
            UserId = userId;
        }
    }
}
