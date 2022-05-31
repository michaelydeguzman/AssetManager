using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class UpdateAssetRequest : IRequest<HandlerResult<UpdateAssetDto>>
    {
        public string UserId { get; private set; }
        public UpdateAssetDto AssetDto { get; private set; }

        public UpdateAssetRequest(UpdateAssetDto asset, string userId)
        {
            AssetDto = asset;
            UserId = userId;
        }
    }
}