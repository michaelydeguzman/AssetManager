using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class UpdateAssetStatusRequest : IRequest<HandlerResult<UpdateAssetStatusDto>>
    {
        public string UserId { get; private set; }

        public UpdateAssetStatusDto Assets { get; private set; }

        public UpdateAssetStatusRequest(UpdateAssetStatusDto assets, string userId)
        {
            Assets = assets;
            UserId = userId;
        }
    }
}