using DAM.Application.Assets.Dtos;
using DAM.Application.Pin.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Pin.Requests
{
    public class PinAssetRequest : IRequest<HandlerResult<IEnumerable<PinAssetsDto>>>
    {
        public PinAssetRequest()
        {
        }
        public class GetPinAssetsRequest : IRequest<HandlerResult<IEnumerable<PinAssetsDto>>>
        {
            public string UserId { get; private set; }

            public GetPinAssetsRequest(string userId)
            {
                UserId = userId;
            }
        }
        public class AddPinAssetsRequest : IRequest<HandlerResult<IEnumerable<PinAssetsDto>>>
        {
            public PinAssetsDto PinObj { get; private set; }

            public AddPinAssetsRequest(PinAssetsDto pinObj)
            {
                PinObj = pinObj;
            }
        }
        public class RemovePinAssetsRequest : IRequest<HandlerResult<IEnumerable<PinAssetsDto>>>
        {
            public PinAssetsDto PinObj { get; private set; }

            public RemovePinAssetsRequest(PinAssetsDto pinObj)
            {
                PinObj = pinObj;
            }
        }
        public class OrderPinAssetsRequest : IRequest<HandlerResult<IEnumerable<PinAssetsDto>>>
        {
            public PinAssetsDto PinObj { get; private set; }

            public OrderPinAssetsRequest(PinAssetsDto pinObj)
            {
                PinObj = pinObj;
            }
        }
        public class GetPinAssetsDetailRequest : IRequest<HandlerResult<IEnumerable<AssetDto>>>
        {
            public string UserId { get; private set; }

            public GetPinAssetsDetailRequest(string userId)
            {
                UserId = userId;
            }
        }
    }
}
