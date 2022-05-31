using DAM.Application.Approval.Dtos;
using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Requests
{
    public class GetMySentApprovalAssetsRequest : IRequest<HandlerResult<IEnumerable<AssetDto>>>
    {
        public string UserId { get; private set; }
        public int AssetVersionId { get; private set; }
        public GetMySentApprovalAssetsRequest(string userId)
        {
            UserId = userId;
        }
    }
}
