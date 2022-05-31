using DAM.Application.Assets.Dtos;
using DAM.Application.Pin.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Pin.Requests
{
    public class PinFolderRequest : IRequest<HandlerResult<IEnumerable<PinFoldersDto>>>
    {
        public PinFolderRequest()
        {
        }
        public class GetPinFoldersRequest : IRequest<HandlerResult<IEnumerable<PinFoldersDto>>>
        {
            public string UserId { get; private set; }

            public GetPinFoldersRequest(string userId)
            {
                UserId = userId;
            }
        }
        public class AddPinFoldersRequest : IRequest<HandlerResult<IEnumerable<PinFoldersDto>>>
        {
            public PinFoldersDto PinObj { get; private set; }

            public AddPinFoldersRequest(PinFoldersDto pinObj)
            {
                PinObj = pinObj;
            }
        }
        public class RemovePinFoldersRequest : IRequest<HandlerResult<IEnumerable<PinFoldersDto>>>
        {
            public PinFoldersDto PinObj { get; private set; }

            public RemovePinFoldersRequest(PinFoldersDto pinObj)
            {
                PinObj = pinObj;
            }
        }
        public class OrderPinFoldersRequest : IRequest<HandlerResult<IEnumerable<PinFoldersDto>>>
        {
            public PinFoldersDto PinObj { get; private set; }

            public OrderPinFoldersRequest(PinFoldersDto pinObj)
            {
                PinObj = pinObj;
            }
        }
        public class GetPinFoldersDetailRequest : IRequest<HandlerResult<IEnumerable<AssetDto>>>
        {
            public string UserId { get; private set; }

            public GetPinFoldersDetailRequest(string userId)
            {
                UserId = userId;
            }
        }
    }
}
