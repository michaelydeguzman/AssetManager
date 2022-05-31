using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Requests
{
    public class GetAssetsForApprovalRequest : IRequest<HandlerResult<IEnumerable<AssetDto>>>
    {
        public string UserId { get; private set; }
        public GetAssetsForApprovalRequest(string userId) 
        {
            UserId = userId;
        }
    }
}
