using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;

namespace DAM.Application.Assets.Requests
{
    public class AddAssetPackageRequest : IRequest<HandlerResult<AssetPackageDto>>
    {

        public AssetPackageDto AssetPackageDto { get; private set; }

        public string UserId { get; private set; }

        public AddAssetPackageRequest(AssetPackageDto asset, string userId)
        {
            AssetPackageDto = asset;
            UserId = userId;
        }
    }
}
