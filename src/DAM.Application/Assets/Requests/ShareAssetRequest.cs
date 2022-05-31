using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Requests
{
    public class ShareAssetRequest : IRequest<HandlerResult<ShareDto>>
    {
        public ShareDto ShareDto { get; private set; }

        public ShareAssetRequest(ShareDto share)
        {
            ShareDto = share;
        }
    }
}
