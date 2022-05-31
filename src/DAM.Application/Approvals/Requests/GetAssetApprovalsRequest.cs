using DAM.Application.Approval.Dtos;
using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Requests
{
    public class GetAssetApprovalsRequest : IRequest<HandlerResult<IEnumerable<ApprovalLevelDto>>>
    {
        public int AssetId { get; private set; }
        public int AssetVersionId { get; private set; }
        public GetAssetApprovalsRequest(int assetId, int assetVersionId)
        {
            AssetId = assetId;
            AssetVersionId = assetVersionId;
        }
    }
}
